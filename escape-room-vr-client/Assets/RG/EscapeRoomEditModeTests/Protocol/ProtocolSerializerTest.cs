using System.IO;
using NUnit.Framework;
using RG.EscapeRoom.Model.Math;
using RG.EscapeRoomProtocol;
using RG.EscapeRoomProtocol.Messages;

namespace RG.EscapeRoomEditModeTests.Protocol
{
    public class ProtocolSerializerTest
    {
        private ProtocolSerializer protocolSerializer;
        private TestMessageReceiver testMessageReceiver;
        private ByteFifoBuffer byteFifoBuffer;
        private PrimitiveSerializer primitiveSerializer;

        [SetUp]
        public void SetUp()
        {
            byteFifoBuffer = new ByteFifoBuffer(1024);
            testMessageReceiver = new TestMessageReceiver();
            primitiveSerializer = new PrimitiveSerializer();
            protocolSerializer = new ProtocolSerializer(primitiveSerializer);
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

        [Test]
        public void TestMessageSerializeThenDeserializePlayerPositionMessage()
        {
            var headPosition = new Vector3(1,2,3);
            var leftHandPosition = new Vector3(4,5,6);
            var rightHandPosition = new Vector3(7,8,9);
            var headRotation = new Quaternion(1,2,3,4);
            var leftHandRotation = new Quaternion(5,6,7,8);
            var rightHandRotation = new Quaternion(9,1,2,3);
            var message = new PlayerPositionMessage(new PlayerMessageBase(1234),
                headPosition, headRotation,
                leftHandPosition, leftHandRotation,
                rightHandPosition, rightHandRotation);
            protocolSerializer.SerializeMessage(message, byteFifoBuffer);
            protocolSerializer.DeserializeNextMessage(byteFifoBuffer, testMessageReceiver);
            var deserializedMessage = (PlayerPositionMessage) testMessageReceiver.messages[0];
            Assert.AreEqual(1234, deserializedMessage.playerMessageBase.senderId);
            Assert.AreEqual(headPosition, deserializedMessage.headPosition);
            Assert.AreEqual(rightHandPosition, deserializedMessage.rightHandPosition);
        }

        [Test]
        public void SerializeFloatWritesCorrect4Bytes()
        {
            var fifoBuffer = new ByteFifoBuffer(16);
            primitiveSerializer.Write(1f, fifoBuffer);
            Assert.AreEqual(4, fifoBuffer.Length);
        }
    }
}