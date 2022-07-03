namespace RG.EscapeRoomProtocol.Messages
{
    public struct ClientConnectMessage
    {
        public byte[] accessToken;

        public ClientConnectMessage(byte[] accessToken)
        {
            this.accessToken = accessToken;
        }
    }
}