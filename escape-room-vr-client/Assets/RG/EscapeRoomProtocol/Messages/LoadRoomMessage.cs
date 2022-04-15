namespace RG.EscapeRoomProtocol.Messages
{
    public struct LoadRoomMessage
    {
        public const ushort ID = 1;
        public string roomId;

        public LoadRoomMessage(string roomId)
        {
            this.roomId = roomId;
        }
    }
}