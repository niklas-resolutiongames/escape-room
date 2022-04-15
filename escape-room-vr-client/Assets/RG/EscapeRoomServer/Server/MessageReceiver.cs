using RG.EscapeRoom.Model.Rooms;
using RG.EscapeRoomProtocol;
using RG.EscapeRoomProtocol.Messages;

namespace RG.EscapeRoomServer.Server
{
    public class ClientMessageReceiver : MessageReceiver
    {
        private readonly Client client;
        private readonly MessageSender messageSender;
        private readonly RoomDefinition roomDefinition;

        public ClientMessageReceiver(Client client, MessageSender messageSender, RoomDefinition roomDefinition)
        {
            this.client = client;
            this.messageSender = messageSender;
            this.roomDefinition = roomDefinition;
        }

        public void Receive(ClientConnectMessage message)
        {
            messageSender.SendMessage(client, new LoadRoomMessage(roomDefinition.roomDefinitionId));
        }

        public void Receive(LoadRoomMessage message)
        {
            
        }
    }
}