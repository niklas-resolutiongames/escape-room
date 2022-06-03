namespace RG.EscapeRoomProtocol.Messages
{
    public struct PlayerMessageBase
    {
        public int senderId;

        public PlayerMessageBase(int senderId)
        {
            this.senderId = senderId;
        }
    }
}