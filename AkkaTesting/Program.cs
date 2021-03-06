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
            services.AddSingleton<Actor1>();
            services.AddAllActorsFromAssemblyOf<Program>();
            services.AddData();
        }

        public static int _totalInsert = 100;
        public static int _timesToRun = 8;
        public static int _actorInserted;

        public static async Task Run()
        {
            var system = GetService<ActorSystem>();

            _log.Information("0 - EXIT | 1 - Job Actor | 2 - Insert Actor | 3 - EventStream");
            IActorRef luiz = null, jobActor = null, actor1 = null;

            while (true)
            {
                string whatIsGoingToDo = Console.ReadLine();

                if (whatIsGoingToDo.Equals("0"))
                {
                    break;
                }

                if (whatIsGoingToDo.Equals("1"))
                {
                    if (jobActor == null)
                    {
                        jobActor = system.ActorOf(DIProps.Create<JobActor>());
                    }
                }
                else if (whatIsGoingToDo.Equals("2"))
                {

                    if (luiz == null)
                    {
                        luiz = system.ActorOf(DIProps.Create<Luiz>(), "luiz");
                    }

                    //var repository = GetService<IClientRepository>();

                    //Stopwatch sw = new Stopwatch();
                    //sw.Start();

                    //_log.Information("Começando a medir o tempo sem ator");

                    //for (int i = 0; i < _timesToRun; i++)
                    //{
                    //    for (int j = 0; j < _totalInsert; j++)
                    //    {
                    //        var client = new Client()
                    //        {
                    //            Id = Guid.NewGuid(),
                    //            City = "BH" + i,
                    //            CPF = "01" + i,
                    //            Name = "Nome" + i
                    //        };
                    //        await Task.Delay(200);
                    //        await repository.SaveAsync(client);
                    //    }

                    //}
                    //sw.Stop();
                    //_log.Information("Tempo sem ator: {0}", sw.Elapsed);
                    luiz.Tell(new InitializeActor(8));
                }
                else if (whatIsGoingToDo.Equals("3"))
                {
                    if (actor1 == null)
                    {
                        actor1 = system.ActorOf(DIProps.Create<Actor1>(), "actor1");
                        system.ActorOf(DIProps.Create<Actor2>(), "actor2");
                        system.ActorOf(DIProps.Create<Actor3>(), "actor3");
                        system.ActorOf(DIProps.Create<Actor4>(), "actor4");
                        system.ActorOf(DIProps.Create<Actor5>(), "actor5");
                        system.ActorOf(DIProps.Create<Actor6>(), "actor6");

                        actor1.Tell(new InitializeActor());
                    }
                    else
                    {
                        actor1.Tell(new DoItAgain());
                    }
                }
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

                    var props = new RoundRobinPool(m.WorkerCounter).Props(Props.Create<Worker>());
                    _worker = Context.ActorOf(props, "worker" + m.WorkerCounter);
                    _currentWorkers = m.WorkerCounter;

                    for (int i = 0; i < _totalInsert * _timesToRun; i++)
                    {
                        Message msg = new Message("MSG" + i.ToString());

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


                    //if (_currentWorkers < _workerMax)
                    //{
                    //    Self.Tell(new InitializeActor(_currentWorkers + 5));
                    //}
                });
            }
        }

        public class Worker : ReceiveActor
        {
            private readonly Guid _myId = Guid.NewGuid();
            public Worker()
            {
                //ReceiveAsync<Message>(async m =>
                //{
                //    _log.Information("{0} Executando 10", _myId);
                //    await ProccessMessage(m);
                //    _log.Information("{0} Executado 10", _myId);

                //});

                Receive<Message>(m =>
                {

                    var t = new Task<Message>(() =>
                    {
                        return m;
                    });

                    _log.Information("{0} Executando 15 | thread: {1}",_actorInserted,  t.Id);
                    ProccessMessage(t).GetAwaiter().GetResult();
                    _log.Information("{0} Executado 15 | thread: {1}", _actorInserted, t.Id);


                });

            }


            public async Task ProccessMessage(Task<Message> msg)
            {
                int tries = 0, success = 0;
                var repository = GetService<IClientRepository>();

                do
                {
                    try
                    {
                        _log.Information("Taskid Método : {0}", msg.Id);
                        await Task.Delay(200);
                        var msgToSave = await msg;
                        success = await repository.SaveAsync(msgToSave.Client);
                        ////Simulação de qualquer validação que vai ser feita antes de inserir no banco.
                        //await Task.Delay(RandomEx.Next(TimeSpan.FromMilliseconds(50), TimeSpan.FromMilliseconds(1500)));

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

