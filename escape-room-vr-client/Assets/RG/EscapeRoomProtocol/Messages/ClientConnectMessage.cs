namespace RG.EscapeRoomProtocol.Messages
{
    public struct ClientConnectMessage
    {
        public const ushort ID = 0;
        public byte[] accessToken;

        public ClientConnectMessage(byte[] accessToken)
        {
            this.accessToken = accessToken;
        }
    }
}