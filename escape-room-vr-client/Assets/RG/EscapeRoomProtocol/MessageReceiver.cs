using RG.EscapeRoomProtocol.Messages;

namespace RG.EscapeRoomProtocol
{
    public interface MessageReceiver
    {
        void Receive(ClientConnectMessage message);
        void Receive(LoadRoomMessage message);
        void Receive(RequestGrabMessage message);
        void Receive(GrabResultMessage message);
        void Receive(ClientWelcomeMessage message);
        void Receive(PlayerPositionMessage message);
        void MessageDiscarded(ushort nextMessageType);
    }
}