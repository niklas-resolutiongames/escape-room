using System.IO;
using System.Net.Sockets;
using RG.EscapeRoomProtocol;
using RG.EscapeRoomProtocol.Messages;

namespace RG.EscapeRoomServer.Server
{
    public class UdpMessageSender : MessageSender
    {
        private readonly Socket socket;
        private readonly ProtocolSerializer protocolSerializer;
        private ByteFifoBuffer byteBuffer;

        public UdpMessageSender(Socket socket, ProtocolSerializer protocolSerializer)
        {
            this.socket = socket;
            this.protocolSerializer = protocolSerializer;
        }

        public void Init()
        {
            byteBuffer = new ByteFifoBuffer(1024);
        }

        public void SendMessage(Client client, LoadRoomMessage loadRoomMessage)
        {
            lock (byteBuffer)
            {
                int numberOfBytes = protocolSerializer.SerializeMessage(loadRoomMessage, byteBuffer);
                socket.SendTo(byteBuffer.ReadAllAsArray(), numberOfBytes, SocketFlags.None, client.endPoint);
            }
        }
    }

    public interface MessageSender
    {
        void SendMessage(Client client, LoadRoomMessage loadRoomMessage);
    }
}