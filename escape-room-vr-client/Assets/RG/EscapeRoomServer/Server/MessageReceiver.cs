using RG.EscapeRoom.Model.Rooms;
using RG.EscapeRoomProtocol;
using RG.EscapeRoomProtocol.Messages;

namespace RG.EscapeRoomServer.Server
{
    public class ServerMessageReceiver : MessageReceiver
    {
        private readonly Client client;
        private readonly MessageSender messageSender;
        private readonly RoomDefinition roomDefinition;
        private int nextPlayerNetworkId;

        public ServerMessageReceiver(Client client, MessageSender messageSender, RoomDefinition roomDefinition)
        {
            this.client = client;
            this.messageSender = messageSender;
            this.roomDefinition = roomDefinition;
        }

        public void Receive(ClientConnectMessage message)
        {
            messageSender.SendMessage(client, new ClientWelcomeMessage(nextPlayerNetworkId++));
            messageSender.SendMessage(client, new LoadRoomMessage(roomDefinition.roomDefinitionId));
        }

        public void Receive(LoadRoomMessage message)
        {
            
        }

        public void Receive(RequestGrabMessage message)
        {
            messageSender.Broadcast(new GrabResultMessage(message, true));
        }

        public void Receive(GrabResultMessage message)
        {
        }

        public void Receive(ClientWelcomeMessage message)
        {
            
        }
    }
}