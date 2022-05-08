namespace RG.EscapeRoomProtocol.Messages
{
    public struct LoadRoomMessage
    {
        public const ushort ID = 1;
        public string roomDefinitionId;

        public LoadRoomMessage(string roomDefinitionId)
        {
            this.roomDefinitionId = roomDefinitionId;
        }
    }

}