namespace RG.EscapeRoomProtocol.Messages
{
    public struct LoadRoomMessage
    {
        public string roomDefinitionId;

        public LoadRoomMessage(string roomDefinitionId)
        {
            this.roomDefinitionId = roomDefinitionId;
        }
    }

}