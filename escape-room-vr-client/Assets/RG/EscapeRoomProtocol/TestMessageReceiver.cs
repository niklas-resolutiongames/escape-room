using System;
using System.Collections.Generic;
using RG.EscapeRoomProtocol.Messages;

namespace RG.EscapeRoomProtocol
{


    public class TestMessageReceiver : MessageReceiver
    {
        public List<object> messages = new List<object>();
        
        public void Receive(ClientConnectMessage message)
        {
            messages.Add(message);
        }

        public void Receive(LoadRoomMessage message)
        {
            messages.Add(message);
        }

        public void Receive(RequestGrabMessage message)
        {
            messages.Add(message);
        }

        public void Receive(GrabResultMessage message)
        {
            messages.Add(message);
        }

        public void Receive(ClientWelcomeMessage message)
        {
            messages.Add(message);   
        }

        public void Receive(PlayerPositionMessage message)
        {
            messages.Add(message);
        }

        public void MessageDiscarded(ushort messageType)
        {
            throw new Exception($"No messages should be discarded, but one of id {messageType} was");
        }
    }
}