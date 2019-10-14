using Akka.Actor;
using Serilog;
using System;
using System.Threading.Tasks;

namespace AkkaTesting.Actor
{
    public class JobActor : ReceiveActor
    {
        private ICancelable _schedule;
        private TimeSpan _timeToRun = TimeSpan.FromSeconds(10);
        private ILogger _log = Log.ForContext<JobActor>();

        public JobActor()
        {
            Become(Running);
        }

        void Running()
        {
            ScheduleNextRun(Run.Instance, TimeSpan.Zero);

            ReceiveAsync<Run>(async m =>
            {
                await Task.Delay(50);
                _log.Information("Running Job");
                ScheduleNextRun(Run.Instance, _timeToRun);
            });
        }


        void ScheduleNextRun(object message, TimeSpan delay)
        {
            //se tiver agendamento anterior, ele cancela para rodar  o novo.
            _schedule.CancelIfNotNull();
            _schedule = Context.System.Scheduler.ScheduleTellOnceCancelable(delay, Self, message, Self);
        }

        #region [ CLASSES ]
        public class Run
        {
            public static readonly Run Instance = new Run();
        }
        #endregion
    }
}
