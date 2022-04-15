using System.IO;
using NUnit.Framework;
using RG.EscapeRoomProtocol;
using RG.EscapeRoomProtocol.Messages;

namespace RG.EscapeRoomEditModeTests.Protocol
{
    public class ProtocolSerializerTest
    {
        private ProtocolSerializer protocolSerializer;
        private MemoryStream writeStream;
        private TestMessageReceiver testMessageReceiver;
        private MemoryStream readStream;

        [SetUp]
        public void SetUp()
        {
            var buffer = new byte[1024];
            writeStream = new MemoryStream(buffer);
            readStream = new MemoryStream(buffer);
            testMessageReceiver = new TestMessageReceiver();
            protocolSerializer = new ProtocolSerializer();
        }

        [Test]
        public void TestMessageSerializeThenDeserializeLoadRoomMessage()
        {
            string roomId = "abc123";
            var message = new LoadRoomMessage(roomId);
            protocolSerializer.SerializeMessage(message, writeStream);
            protocolSerializer.DeserializeNextMessage(readStream, testMessageReceiver);
            var deserializedMessage = (LoadRoomMessage) testMessageReceiver.messages[0];
            Assert.AreEqual(roomId, deserializedMessage.roomDefinitionId);
        }
    }
}