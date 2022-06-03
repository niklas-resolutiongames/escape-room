using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
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
            messageSender.SocketConnected(client, serverEndPoint);
        }
        
        
    }

    public class MessageSender:ITickable
    {
        private readonly ProtocolSerializer protocolSerializer;
        private readonly IncomingMessagesData incomingMessagesData;
        
        ByteFifoBuffer scratchStream;
        private UdpClient client;
        private IPEndPoint serverEndPoint;
        private PlayerMessageBase playerMessageBase = new PlayerMessageBase();

        public MessageSender(ProtocolSerializer protocolSerializer, IncomingMessagesData incomingMessagesData)
        {
            this.protocolSerializer = protocolSerializer;
            this.incomingMessagesData = incomingMessagesData;
        }

        public void Init()
        {
            scratchStream = new ByteFifoBuffer(1024);
        }

        public void SocketConnected(UdpClient client, IPEndPoint serverEndPoint)
        {
            this.client = client;
            this.serverEndPoint = serverEndPoint;
            var message = new ClientConnectMessage(Array.Empty<byte>());
            var numberOfBytes = protocolSerializer.SerializeMessage(message, scratchStream);
            
            client.SendAsync(scratchStream.ReadAllAsArray(), numberOfBytes, serverEndPoint);
        }

        public void SendRequestGrabMessage(byte hand, string grabbableId, bool isGrab)
        {
            var message = new RequestGrabMessage(playerMessageBase, hand, grabbableId, isGrab);
            var numberOfBytes = protocolSerializer.SerializeMessage(message, scratchStream);
            
            client.SendAsync(scratchStream.ReadAllAsArray(), numberOfBytes, serverEndPoint);
        }

        public void Tick()
        {
            while (incomingMessagesData.welcomeMessages.Count > 0)
            {
                var welcomeMessage = incomingMessagesData.welcomeMessages.Dequeue();
                playerMessageBase.senderId = welcomeMessage.playerNetworkId;
            }
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

        public void Receive(ClientWelcomeMessage message)
        {
            incomingMessagesData.welcomeMessages.Enqueue(message);
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
    }
}