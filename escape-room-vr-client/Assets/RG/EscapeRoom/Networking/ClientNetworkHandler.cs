using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using RG.EscapeRoom.Wiring;
using RG.EscapeRoomProtocol;
using RG.EscapeRoomProtocol.Messages;

namespace RG.EscapeRoom.Networking
{
    public class ClientNetworkHandler:ITickable, IDisposable
    {
        
        private readonly MessageSender messageSender;
        private readonly ProtocolSerializer protocolSerializer;
        private readonly MessageReceiver messageReceiver;
        
        private IPEndPoint serverEndPoint;
        private IPEndPoint clientEndPoint;
        private UdpClient client;
        private ByteFifoBuffer byteFifoBuffer = new ByteFifoBuffer(1024);

        public ClientNetworkHandler(MessageSender messageSender, ProtocolSerializer protocolSerializer, MessageReceiver messageReceiver)
        {
            this.messageSender = messageSender;
            this.protocolSerializer = protocolSerializer;
            this.messageReceiver = messageReceiver;
        }

        public void Tick()
        {
            CheckSocketForIncomingData();
            CheckReadDataForMessages();
        }

        private void CheckReadDataForMessages()
        {
            while (byteFifoBuffer.Length > 0)
            {
                protocolSerializer.DeserializeNextMessage(byteFifoBuffer, messageReceiver);
            }
        }

        private void CheckSocketForIncomingData()
        {
            if (client.Available > 0)
            {
                var data = client.Receive(ref serverEndPoint);
                byteFifoBuffer.Write(data,0,data.Length);
            }
        }

        public void Connect(int serverPort, string serverIp)
        {
            serverEndPoint = new IPEndPoint(IPAddress.Parse(serverIp), serverPort);
            clientEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), serverPort+1);
            client = new UdpClient(clientEndPoint);
            messageSender.SocketConnected(client, serverEndPoint);
        }


        public void Dispose()
        {
            client.Dispose();
        }
    }
}