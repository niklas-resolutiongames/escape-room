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
            
        }
    }
}