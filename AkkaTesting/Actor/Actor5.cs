using Akka.Actor;
using AkkaTesting.Messages;
using Serilog;

namespace AkkaTesting.Actor
{
    public class Actor5 : ReceiveActor
    {
        private ILogger _log = Log.ForContext<Actor5>();
        public Actor5()
        {
            Context.System.EventStream.Subscribe(Self, typeof(BroadcastRequest));

            Receive<BroadcastRequest>(msg =>
            {
                _log.Information("Actor5 Receiving a request from {0}", Sender.Path);
                Sender.Tell(new BroadcastResponse());
            });
        }
    }
}
