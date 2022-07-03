using System;
using System.Net;
using System.Net.Sockets;
using RG.EscapeRoom.Model.Math;
using RG.EscapeRoom.Wiring;
using RG.EscapeRoomProtocol;
using RG.EscapeRoomProtocol.Messages;

namespace RG.EscapeRoom.Networking
{
    public class MessageSender:ITickable
    {
        private readonly ProtocolSerializer protocolSerializer;
        private readonly IncomingMessagesData incomingMessagesData;
        private readonly ReceivedNetworkStateData receivedNetworkStateData;
        
        ByteFifoBuffer scratchStream;
        private UdpClient client;
        private IPEndPoint serverEndPoint;
        private PlayerMessageBase playerMessageBase = new PlayerMessageBase();

        public MessageSender(ProtocolSerializer protocolSerializer, IncomingMessagesData incomingMessagesData, ReceivedNetworkStateData receivedNetworkStateData)
        {
            this.protocolSerializer = protocolSerializer;
            this.incomingMessagesData = incomingMessagesData;
            this.receivedNetworkStateData = receivedNetworkStateData;
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
                receivedNetworkStateData.localPlayerNetworkId = welcomeMessage.playerNetworkId;
            }
        }

        public void SendPlayerPositionMessage(Vector3 headPosition, Quaternion headRotation, Vector3 leftHandPosition, Quaternion leftHandRotation, Vector3 rightHandPosition, Quaternion rightHandRotation)
        {
            var message = new PlayerPositionMessage(playerMessageBase, headPosition, headRotation, leftHandPosition, leftHandRotation,
                rightHandPosition, rightHandRotation);
            
            var numberOfBytes = protocolSerializer.SerializeMessage(message, scratchStream);
            
            client.SendAsync(scratchStream.ReadAllAsArray(), numberOfBytes, serverEndPoint);
        }
    }
}