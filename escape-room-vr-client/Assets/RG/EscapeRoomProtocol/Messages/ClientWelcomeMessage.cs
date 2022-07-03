namespace RG.EscapeRoomProtocol.Messages
{
    public struct ClientWelcomeMessage
    {
        public int playerNetworkId;

        public ClientWelcomeMessage(int playerNetworkId)
        {
            this.playerNetworkId = playerNetworkId;
        }
    }
}