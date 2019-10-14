﻿using Akka.Actor;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using Serilog;
using AkkaTesting.Messages;
using Akka.Routing;
using AkkaTesting.Data;
using AkkaTesting.Domain.Entity;
using AkkaTesting.Domain.Interfaces.Repository;
using AkkaTesting.Infra.Helper;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using AkkaTesting.Actor;

namespace AkkaTesting
{
    public partial class Program
    {
        private static ILogger _log = Log.ForContext<Program>();

        public static void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(ctx => ActorSystem.Create("app"));
            services.AddAllActorsFromAssemblyOf<Program>();
            services.AddData();           
        }

        public static int _totalInsert = 50;
        public static int _timesToRun = 5;
        public static int _actorInserted;

        public static async Task Run()
        {
            var system = GetService<ActorSystem>();

            _log.Information("1 - Job Actor | 2 - Insert Actor");

            string whatIsGoingToDo = Console.ReadLine();

            if(whatIsGoingToDo.Equals("1"))
            {
                system.ActorOf(DIProps.Create<JobActor>());
            }
            else
            {
                var luiz = system.ActorOf(DIProps.Create<Luiz>(), "luiz");
                var repository = GetService<IClientRepository>();

                Stopwatch sw = new Stopwatch();
                sw.Start();

                for (int i = 0; i < _timesToRun; i++)
                {
                    for (int j = 0; j < _totalInsert; j++)
                    {
                        var client = new Client()
                        {
                            Id = Guid.NewGuid(),
                            City = "BH" + i,
                            CPF = "01" + i,
                            Name = "Nome" + i
                        };
                        await Task.Delay(200);
                        await repository.SaveAsync(client);
                    }

                }
                sw.Stop();
                _log.Information("Tempo sem ator: {0}", sw.Elapsed);
                luiz.Tell(new InitializeActor(2));
            }
        }

        #region [ ACTORS ]
        public class Luiz : ReceiveActor
        {
            IActorRef _worker;
            private Stopwatch sw = new Stopwatch();
            private int _workerMax = 31, _currentWorkers = 0;
            public Luiz()
            {
                Receive<InitializeActor>(m =>
                {
                    _log.Information("[Luiz] - Inicializando");
                    sw.Restart();

                    var workerProps = DIProps.Create<Worker>().WithRouter(new SmallestMailboxPool(m.WorkerCounter));
                    _worker = Context.ActorOf(workerProps, "worker" + m.WorkerCounter);
                    _currentWorkers = m.WorkerCounter;

                    for (int i = 0; i < _totalInsert * _timesToRun; i++)
                    {
                        Mensagem msg = new Mensagem("MSG" + i.ToString());

                        msg.Client = new Client()
                        {
                            Id = Guid.NewGuid(),
                            City = "BH" + i,
                            CPF = "01" + i,
                            Name = "Nome" + i
                        };
                        _worker.Tell(msg);
                    }
                });

                Receive<EndActor>(m =>
                {
                    sw.Stop();
                    _log.Information("{0} Atores | Tempo gasto no sistema de atores: {1}", _currentWorkers, sw.Elapsed);
                    _actorInserted = 0;


                    if(_currentWorkers < _workerMax)
                    {
                        Self.Tell(new InitializeActor(_currentWorkers + 5));
                    }
                });
            }
        }

        public class Worker : ReceiveActor
        {
            public Worker()
            {
                ReceiveAsync<Mensagem>(async m =>
                {
                    await ProccessMessage(m);
                });
            }

            public async Task ProccessMessage(Mensagem msg)
            {
                var repository = GetService<IClientRepository>();
                int tries = 0, success = 0;
                do
                {
                    try
                    {
                        await Task.Delay(200);

                        ////Simulação de qualquer validação que vai ser feita antes de inserir no banco.
                        //await Task.Delay(RandomEx.Next(TimeSpan.FromMilliseconds(50), TimeSpan.FromMilliseconds(1500)));
                        success = await repository.SaveAsync(msg.Client);

                        if (success > 0)
                        {
                            Interlocked.Increment(ref _actorInserted);

                            if (ImTheLast)
                            {
                                Sender.Tell(EndActor.Instance);
                            }

                            break;
                        }
                        await Task.Delay(25);
                    }
                    catch (Exception e)
                    {
                        tries++;
                        await Task.Delay(25);
                    }

                } while (tries < 25);

                //_log.Information("{0} | Success: {1} | Tries: {2} | Client: {3}", DateTime.UtcNow.ToLongTimeString()
                //    , success, tries, msg.Client.Name);
            }

            private bool ImTheLast => Interlocked.Equals(_actorInserted, _totalInsert * _timesToRun);        
        }
        #endregion
    }
}

