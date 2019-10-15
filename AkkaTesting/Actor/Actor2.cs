using Akka.Actor;
using AkkaTesting.Messages;
using Serilog;

namespace AkkaTesting.Actor
{
    public class Actor2 : ReceiveActor
    {
        private ILogger _log = Log.ForContext<Actor2>();
        public Actor2()
        {
            Context.System.EventStream.Subscribe(Self, typeof(BroadcastRequest));

            Receive<BroadcastRequest>(msg =>
            {
                _log.Information("Actor2 Receiving a request from {0}", Sender.Path);
                Sender.Tell(new BroadcastResponse());
            });
        }
    }
}
