using System.Collections.Generic;
using RG.EscapeRoomProtocol;
using RG.EscapeRoomProtocol.Messages;

namespace RG.EscapeRoom.Networking
{
    public class ClientMessageReceiver: MessageReceiver
    {
        private readonly IncomingMessagesData incomingMessagesData;

        public ClientMessageReceiver(IncomingMessagesData incomingMessagesData)
        {
            this.incomingMessagesData = incomingMessagesData;
        }

        public void Receive(ClientConnectMessage message)
        {
        }

        public void Receive(ClientWelcomeMessage message)
        {
            incomingMessagesData.welcomeMessages.Enqueue(message);
        }

        public void Receive(PlayerPositionMessage message)
        {
            incomingMessagesData.playerPositionMessages.Enqueue(message);
        }

        public void MessageDiscarded(ushort messageType)
        {
            
        }

        public void Receive(LoadRoomMessage message)
        {
            incomingMessagesData.loadRoomMessages.Enqueue(message);
        }

        public void Receive(RequestGrabMessage message)
        {
        }

        public void Receive(GrabResultMessage message)
        {
            incomingMessagesData.incomingGrabResults.Enqueue(message);
        }
    }
    
    public class IncomingMessagesData
    {
        public Queue<LoadRoomMessage> loadRoomMessages = new Queue<LoadRoomMessage>();
        public Queue<GrabResultMessage> incomingGrabResults = new Queue<GrabResultMessage>();
        public Queue<ClientWelcomeMessage> welcomeMessages = new Queue<ClientWelcomeMessage>();
        public Queue<PlayerPositionMessage> playerPositionMessages = new Queue<PlayerPositionMessage>();
    }
}