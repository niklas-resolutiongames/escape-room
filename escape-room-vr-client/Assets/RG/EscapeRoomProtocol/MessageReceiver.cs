using RG.EscapeRoomProtocol.Messages;

namespace RG.EscapeRoomProtocol
{
    public interface MessageReceiver
    {
        void Receive(ClientConnectMessage message);
        void Receive(LoadRoomMessage message);
    }
}