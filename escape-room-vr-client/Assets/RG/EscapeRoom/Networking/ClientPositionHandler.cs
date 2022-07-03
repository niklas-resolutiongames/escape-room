using RG.EscapeRoom.Controller.Player;
using RG.EscapeRoom.Wiring;

namespace RG.EscapeRoom.Networking
{
    public class ClientPositionHandler:ITickable
    {
        private readonly MessageSender messageSender;
        private readonly XRPlayerReference playerReference;
        private readonly IncomingMessagesData incomingMessagesData;
        private readonly ReceivedNetworkStateData receivedNetworkStateData;

        public ClientPositionHandler(MessageSender messageSender, XRPlayerReference playerReference, IncomingMessagesData incomingMessagesData, ReceivedNetworkStateData receivedNetworkStateData)
        {
            this.messageSender = messageSender;
            this.playerReference = playerReference;
            this.incomingMessagesData = incomingMessagesData;
            this.receivedNetworkStateData = receivedNetworkStateData;
        }

        public void Tick()
        {
            SendPositionToServer();
            CheckIncomingPositionMessages();
        }

        private void CheckIncomingPositionMessages()
        {
            while (incomingMessagesData.playerPositionMessages.Count > 0)
            {
                var message = incomingMessagesData.playerPositionMessages.Dequeue();
                receivedNetworkStateData.receivedPlayerPositions[message.playerMessageBase.senderId] = message;
            }
        }

        private void SendPositionToServer()
        {
            var headTransform = playerReference.head.transform;
            var leftHandTransform = playerReference.leftHand.transform;
            var rightHandTransform = playerReference.rightHand.transform;
            messageSender.SendPlayerPositionMessage(MathUtils.InternalVector3(headTransform.position),
                MathUtils.InternalQuaternion(headTransform.rotation),
                MathUtils.InternalVector3(leftHandTransform.position), MathUtils.InternalQuaternion(leftHandTransform.rotation),
                MathUtils.InternalVector3(rightHandTransform.position),
                MathUtils.InternalQuaternion(rightHandTransform.rotation));
        }
    }
}