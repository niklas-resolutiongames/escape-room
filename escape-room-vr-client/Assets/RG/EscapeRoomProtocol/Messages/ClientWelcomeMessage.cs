namespace RG.EscapeRoomProtocol.Messages
{
    public struct ClientWelcomeMessage
    {
        public const ushort ID = 4;
        public int playerNetworkId;

        public ClientWelcomeMessage(int playerNetworkId)
        {
            this.playerNetworkId = playerNetworkId;
        }
    }
}