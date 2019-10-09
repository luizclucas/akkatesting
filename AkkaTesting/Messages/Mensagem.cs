using AkkaTesting.Domain.Entity;

namespace AkkaTesting.Messages
{
    public class Mensagem
    {
        public Mensagem(string mensagemASerEnviada)
        {
            MensagemASerEnviada = mensagemASerEnviada;
        }

        public string MensagemASerEnviada { get; set; }

        public Client Client { get; set; }
    }
}
