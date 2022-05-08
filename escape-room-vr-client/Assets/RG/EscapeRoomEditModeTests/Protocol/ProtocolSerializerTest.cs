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
    }
}