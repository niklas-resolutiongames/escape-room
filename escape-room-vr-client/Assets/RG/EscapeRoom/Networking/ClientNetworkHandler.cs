using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using RG.EscapeRoom.Wiring;
using RG.EscapeRoomProtocol;
using RG.EscapeRoomProtocol.Messages;

namespace RG.EscapeRoom.Networking
{
    public class ClientNetworkHandler:ITickable
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
            messageSender.SendMessage(client, serverEndPoint, new ClientConnectMessage(Array.Empty<byte>()));
        }
        
        
    }

    public class MessageSender
    {
        private readonly ProtocolSerializer protocolSerializer;

        ByteFifoBuffer scratchStream;

        public MessageSender(ProtocolSerializer protocolSerializer)
        {
            this.protocolSerializer = protocolSerializer;
        }

        public void Init()
        {
            scratchStream = new ByteFifoBuffer(1024);
        }

        public Task<int> SendMessage(UdpClient client, IPEndPoint serverEndPoint, ClientConnectMessage message)
        {
            var numberOfBytes = protocolSerializer.SerializeMessage(message, scratchStream);
            var sendMessageTask = client.SendAsync(scratchStream.ReadAllAsArray(), numberOfBytes, serverEndPoint);
            return sendMessageTask;
        }
    }
    
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

        public void Receive(LoadRoomMessage message)
        {
            incomingMessagesData.loadRoomMessages.Enqueue(message);
        }
    }

    public class IncomingMessagesData
    {
        public Queue<LoadRoomMessage> loadRoomMessages = new Queue<LoadRoomMessage>();
    }
}