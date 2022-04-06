namespace RG.EscapeRoomProtocol.Messages
{
    public struct ClientHelloMessage
    {
        public byte[] accessToken;

        public ClientHelloMessage(byte[] accessToken)
        {
            this.accessToken = accessToken;
        }
    }
}