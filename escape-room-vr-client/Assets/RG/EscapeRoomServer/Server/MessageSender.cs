using System.Collections.Generic;
using System.Net.Sockets;
using RG.EscapeRoomProtocol;
using RG.EscapeRoomProtocol.Messages;

namespace RG.EscapeRoomServer.Server
{
    public class UdpMessageSender : MessageSender
    {
        private readonly Socket socket;
        private readonly ProtocolSerializer protocolSerializer;
        private readonly HashSet<Client> allConnectedClients;
        private ByteFifoBuffer byteBuffer;

        public UdpMessageSender(Socket socket, ProtocolSerializer protocolSerializer, HashSet<Client> allConnectedClients)
        {
            this.socket = socket;
            this.protocolSerializer = protocolSerializer;
            this.allConnectedClients = allConnectedClients;
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

        public void Broadcast(PlayerPositionMessage message)
        {
            lock (byteBuffer)
            {
                int numberOfBytes = protocolSerializer.SerializeMessage(message, byteBuffer);
                var data = byteBuffer.ReadAllAsArray();
                foreach (var client in allConnectedClients)
                {
                    socket.SendTo(data, numberOfBytes, SocketFlags.None, client.endPoint);
                }
            }

        }

        public void SendMessage(Client client, ClientWelcomeMessage message)
        {
            lock (byteBuffer)
            {
                int numberOfBytes = protocolSerializer.SerializeMessage(message, byteBuffer);
                socket.SendTo(byteBuffer.ReadAllAsArray(), numberOfBytes, SocketFlags.None, client.endPoint);
            }
        }
        public void Broadcast(GrabResultMessage grabResultMessage)
        {
            lock (byteBuffer)
            {
                int numberOfBytes = protocolSerializer.SerializeMessage(grabResultMessage, byteBuffer);
                var data = byteBuffer.ReadAllAsArray();
                foreach (var client in allConnectedClients)
                {
                    socket.SendTo(data, numberOfBytes, SocketFlags.None, client.endPoint);
                }
            }
        }
    }

    public interface MessageSender
    {
        void SendMessage(Client client, LoadRoomMessage loadRoomMessage);
        void Broadcast(GrabResultMessage grabResultMessage);
        void Broadcast(PlayerPositionMessage message);
        void SendMessage(Client client, ClientWelcomeMessage message);
    }
}