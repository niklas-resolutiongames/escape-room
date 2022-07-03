namespace RG.EscapeRoomProtocol.Messages
{
    public struct GrabResultMessage
    {
        public RequestGrabMessage requestMessage;
        public bool wasSuccessful;

        public GrabResultMessage(RequestGrabMessage requestMessage, bool wasSuccessful)
        {
            this.requestMessage = requestMessage;
            this.wasSuccessful = wasSuccessful;
        }
    }
}