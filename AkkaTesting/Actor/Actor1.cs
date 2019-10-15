using Akka.Actor;
using Akka.Event;
using AkkaTesting.Messages;
using Serilog;

namespace AkkaTesting.Actor
{
    public class Actor1 : ReceiveActor
    {
        EventStream ES => Context.System.EventStream;
        private ILogger _log = Log.ForContext<Actor1>();

        public Actor1()
        {
            Receive<InitializeActor>(_ => {
                Become(Runnning);
            });
        }

        public void Runnning()
        {
            ES.Publish(new BroadcastRequest());

            Receive<BroadcastResponse>(msg =>
            {
                _log.Information("Actor1 Receive from {0}", Sender.Path);
            });

            Receive<DoItAgain>(msg =>
            {
                ES.Publish(new BroadcastRequest());
            });
        }
    }
}
