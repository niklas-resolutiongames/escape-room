using NUnit.Framework;
using RG.EscapeRoomProtocol;

namespace RG.EscapeRoomEditModeTests.Protocol
{
    public class ByteFifoBufferTest
    {
        private ByteFifoBuffer byteFifoBuffer;

        [SetUp]
        public void SetUp()
        {
            byteFifoBuffer = new ByteFifoBuffer(1024);
        }

        [Test]
        public void WriteByteToBufferIncreasesLength()
        {
            byteFifoBuffer.WriteByte(3);
            Assert.AreEqual(1, byteFifoBuffer.Length);
        }

        [Test]
        public void WriteArrayToBufferIncreasesLength()
        {
            byteFifoBuffer.Write(new byte[4], 0, 3);
            Assert.AreEqual(3, byteFifoBuffer.Length);
        }

        [Test]
        public void WriteThenReadReturnsCorrectArray()
        {
            for (byte i = 0; i < 10; i++)
            {
                byteFifoBuffer.WriteByte(i);   
            }

            var buffer = new byte[10];
            byteFifoBuffer.Read(buffer, 0, 10);
            
            for (byte i = 0; i < 10; i++)
            {
                var readByte = buffer[i];
                Assert.AreEqual(i, readByte);
            }
        }
    }
}