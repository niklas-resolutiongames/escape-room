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
        private MemoryStream scratchStream;
        private byte[] scratchBuffer;

        public UdpMessageSender(Socket socket, ProtocolSerializer protocolSerializer)
        {
            this.socket = socket;
            this.protocolSerializer = protocolSerializer;
        }

        public void Init()
        {
            scratchBuffer = new byte[1024];
            scratchStream = new MemoryStream(scratchBuffer);
        }

        public void SendMessage(Client client, LoadRoomMessage loadRoomMessage)
        {
            lock (scratchStream)
            {
                int numberOfBytes = protocolSerializer.SerializeMessage(loadRoomMessage, scratchStream);
                socket.SendTo(scratchBuffer, numberOfBytes, SocketFlags.None, client.endPoint);
            }
        }
    }

    public interface MessageSender
    {
        void SendMessage(Client client, LoadRoomMessage loadRoomMessage);
    }
}