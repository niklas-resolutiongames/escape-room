using RG.EscapeRoomProtocol;
using RG.EscapeRoomProtocol.Messages;

namespace RG.EscapeRoomServer.Server
{
    public class ClientMessageReceiver : MessageReceiver
    {
        private readonly Client client;
        private readonly MessageSender messageSender;

        public ClientMessageReceiver(Client client, MessageSender messageSender)
        {
            this.client = client;
            this.messageSender = messageSender;
        }

        public void Receive(ClientConnectMessage message)
        {
            messageSender.SendMessage(client, new LoadRoomMessage("abc123"));
        }

        public void Receive(LoadRoomMessage message)
        {
            
        }
    }
}