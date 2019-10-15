using AkkaTesting.Domain.Entity;

namespace AkkaTesting.Messages
{
    public class Message
    {
        public Message(string messageToBeSend)
        {
            MessageToBeSend = messageToBeSend;
        }

        public string MessageToBeSend { get; set; }

        public Client Client { get; set; }
    }
}
