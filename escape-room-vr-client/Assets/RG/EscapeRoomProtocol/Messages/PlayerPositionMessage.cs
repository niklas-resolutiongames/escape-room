using RG.EscapeRoom.Model.Math;

namespace RG.EscapeRoomProtocol.Messages
{
    public struct PlayerPositionMessage
    {
        public PlayerMessageBase playerMessageBase;
        public Vector3 headPosition;
        public Quaternion headRotation;
        public Vector3 leftHandPosition;
        public Quaternion leftHandRotation;
        public Vector3 rightHandPosition;
        public Quaternion rightHandRotation;

        public PlayerPositionMessage(PlayerMessageBase playerMessageBase, Vector3 headPosition, Quaternion headRotation, Vector3 leftHandPosition, Quaternion leftHandRotation, Vector3 rightHandPosition, Quaternion rightHandRotation)
        {
            this.playerMessageBase = playerMessageBase;
            this.headPosition = headPosition;
            this.headRotation = headRotation;
            this.leftHandPosition = leftHandPosition;
            this.leftHandRotation = leftHandRotation;
            this.rightHandPosition = rightHandPosition;
            this.rightHandRotation = rightHandRotation;
        }
    }
}