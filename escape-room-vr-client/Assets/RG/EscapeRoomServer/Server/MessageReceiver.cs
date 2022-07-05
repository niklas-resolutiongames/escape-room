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
        private readonly ILogger logger;
        private int nextPlayerNetworkId;

        public ServerMessageReceiver(Client client, MessageSender messageSender, RoomDefinition roomDefinition, ILogger logger)
        {
            this.client = client;
            this.messageSender = messageSender;
            this.roomDefinition = roomDefinition;
            this.logger = logger;
        }

        public void Receive(ClientConnectMessage message)
        {
            logger.Info($"Received hello message, assigning player id {nextPlayerNetworkId}");
            messageSender.SendMessage(client, new ClientWelcomeMessage(nextPlayerNetworkId++));
            logger.Info($"Asking client to load  {roomDefinition.roomDefinitionId}");
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

        public void Receive(PlayerPositionMessage message)
        {
            messageSender.Broadcast(message);
        }

        public void MessageDiscarded(ushort messageType)
        {
            logger.Error($"Discarded message of type {messageType}", null);   
        }
    }
}