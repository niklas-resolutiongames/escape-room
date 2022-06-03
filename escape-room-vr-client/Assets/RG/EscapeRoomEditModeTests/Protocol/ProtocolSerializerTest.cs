using System.IO;
using NUnit.Framework;
using RG.EscapeRoomProtocol;
using RG.EscapeRoomProtocol.Messages;

namespace RG.EscapeRoomEditModeTests.Protocol
{
    public class ProtocolSerializerTest
    {
        private ProtocolSerializer protocolSerializer;
        private TestMessageReceiver testMessageReceiver;
        private ByteFifoBuffer byteFifoBuffer;

        [SetUp]
        public void SetUp()
        {
            byteFifoBuffer = new ByteFifoBuffer(1024);
            testMessageReceiver = new TestMessageReceiver();
            protocolSerializer = new ProtocolSerializer();
        }

        [Test]
        public void TestMessageSerializeThenDeserializeLoadRoomMessage()
        {
            string roomId = "abc123";
            var message = new LoadRoomMessage(roomId);
            protocolSerializer.SerializeMessage(message, byteFifoBuffer);
            protocolSerializer.DeserializeNextMessage(byteFifoBuffer, testMessageReceiver);
            var deserializedMessage = (LoadRoomMessage) testMessageReceiver.messages[0];
            Assert.AreEqual(roomId, deserializedMessage.roomDefinitionId);
        }

        [Test]
        public void TestMessageSerializeThenDeserializeRequestGrabMessage()
        {
            var message = new RequestGrabMessage(new PlayerMessageBase(1234), RequestGrabMessage.LeftHand, "12", true);
            protocolSerializer.SerializeMessage(message, byteFifoBuffer);
            protocolSerializer.DeserializeNextMessage(byteFifoBuffer, testMessageReceiver);
            var deserializedMessage = (RequestGrabMessage) testMessageReceiver.messages[0];
            Assert.AreEqual(1234, deserializedMessage.playerMessageBase.senderId);
            Assert.AreEqual(RequestGrabMessage.LeftHand, deserializedMessage.hand);
            Assert.AreEqual("12", deserializedMessage.grabbableId);
            Assert.IsTrue(deserializedMessage.isGrab);
        }

        [Test]
        public void TestMessageSerializeThenDeserializeGrabResultMessage()
        {
            var grabMessage = new RequestGrabMessage(new PlayerMessageBase(1234), RequestGrabMessage.LeftHand, "12", true);
            var message = new GrabResultMessage(grabMessage, true);
            protocolSerializer.SerializeMessage(message, byteFifoBuffer);
            protocolSerializer.DeserializeNextMessage(byteFifoBuffer, testMessageReceiver);
            var deserializedMessage = (GrabResultMessage) testMessageReceiver.messages[0];
            Assert.AreEqual(1234, deserializedMessage.requestMessage.playerMessageBase.senderId);
            Assert.AreEqual(RequestGrabMessage.LeftHand, deserializedMessage.requestMessage.hand);
            Assert.AreEqual("12", deserializedMessage.requestMessage.grabbableId);
            Assert.IsTrue(deserializedMessage.wasSuccessful);
        }
    }
}