namespace RG.EscapeRoomProtocol.Messages
{
    public struct RequestGrabMessage
    {
        public const byte RightHand = 0;
        public const byte LeftHand = 1;
        public PlayerMessageBase playerMessageBase;
        public byte hand;
        public string grabbableId;
        public bool isGrab;

        public RequestGrabMessage(PlayerMessageBase playerMessageBase, byte hand, string grabbableId, bool isGrab)
        {
            this.playerMessageBase = playerMessageBase;
            this.hand = hand;
            this.grabbableId = grabbableId;
            this.isGrab = isGrab;
        }
    }
}