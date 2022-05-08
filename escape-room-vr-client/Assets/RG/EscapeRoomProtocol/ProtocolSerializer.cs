using System.IO;
using System.Text;
using System.Threading.Tasks;
using RG.EscapeRoomProtocol.Messages;

namespace RG.EscapeRoomProtocol
{
    public class ProtocolSerializer
    {

        private ByteFifoBuffer scratchByteFifoBuffer = new ByteFifoBuffer(1024);
        private const int MAX_MESSAGE_SIZE = 1024;

        public int SerializeMessage(ClientConnectMessage message, ByteFifoBuffer byteFifoBuffer)
        {
            int size = 0;
            lock (scratchByteFifoBuffer)
            {
                size += Write(ClientConnectMessage.ID, scratchByteFifoBuffer);
                size += Write(message.accessToken, scratchByteFifoBuffer);
                size += Write((ushort) size, byteFifoBuffer);
                byteFifoBuffer.WriteAll(scratchByteFifoBuffer);
            }

            return size;
        }

        public async Task DeserializeNextMessage(ByteFifoBuffer byteFifoBuffer, MessageReceiver receiver)
        {
            ushort nextMessageLength = await ReadShort(byteFifoBuffer);
            ushort nextMessageType = await ReadShort(byteFifoBuffer);
            switch (nextMessageType) 
            {
                case ClientConnectMessage.ID:
                    await DeserializeHelloMessage(byteFifoBuffer, receiver);
                    break;
                case LoadRoomMessage.ID: 
                    await DeserializeLoadRoomMessage(byteFifoBuffer, receiver);
                    break;
            }
        }

        private async Task DeserializeLoadRoomMessage(ByteFifoBuffer byteFifoBuffer, MessageReceiver receiver)
        {
            byte[] stringBytes = await ReadArray(byteFifoBuffer);
            string roomId = Encoding.UTF8.GetString(stringBytes);
            var message = new LoadRoomMessage(roomId);
            receiver.Receive(message);
        }

        private async Task DeserializeHelloMessage(ByteFifoBuffer byteFifoBuffer, MessageReceiver receiver)
        {
            byte[] accessToken = await ReadArray(byteFifoBuffer);
            var message = new ClientConnectMessage(accessToken);
            receiver.Receive(message);
        }
        
        
        public int SerializeMessage(LoadRoomMessage message, ByteFifoBuffer byteFifoBuffer)
        {
            int size = 0;
            lock (scratchByteFifoBuffer)
            {
                size += Write(LoadRoomMessage.ID, scratchByteFifoBuffer);
                size += Write(message.roomDefinitionId, scratchByteFifoBuffer);
                size += Write((ushort) size, byteFifoBuffer);
                byteFifoBuffer.WriteAll(scratchByteFifoBuffer);
            }

            return size;
   
        }

        private async Task<byte[]> ReadArray(ByteFifoBuffer byteFifoBuffer)
        {
            var length = await ReadShort(byteFifoBuffer);
            await UntilByteFifoBufferContains(byteFifoBuffer, length);
            var array = new byte[length];
            byteFifoBuffer.Read(array,0,  length);
            return array;
        }

        private async Task<ushort> ReadShort(ByteFifoBuffer byteFifoBuffer)
        {
            await UntilByteFifoBufferContains(byteFifoBuffer, 2);
            var hiByte = (byte) byteFifoBuffer.ReadByte();
            var loByte = (byte) byteFifoBuffer.ReadByte();
            return (ushort) ((hiByte << 8) + loByte);
        }

        private async Task UntilByteFifoBufferContains(ByteFifoBuffer byteFifoBuffer, int leastNumberOfExpectedBytes)
        {
            while (byteFifoBuffer.Length < leastNumberOfExpectedBytes)
            {
                await Task.Yield();
            }
        }

        private int Write(ushort s, ByteFifoBuffer byteFifoBuffer)
        {
            var hiByte = (byte) ((s>>8) & 0xff);
            var loByte = (byte) (s & 0xff);
            byteFifoBuffer.WriteByte(hiByte);
            byteFifoBuffer.WriteByte(loByte);
            return 2;
        }
        private int Write(byte[] array, ByteFifoBuffer byteFifoBuffer)
        {
            var arrayLength = array.Length;
            var size = arrayLength;
            size += Write((ushort) arrayLength, byteFifoBuffer);
            byteFifoBuffer.Write(array,0,arrayLength);
            return size;
        }
        private int Write(string s, ByteFifoBuffer byteFifoBuffer)
        {
            return Write(Encoding.UTF8.GetBytes(s), byteFifoBuffer);
        }

    }
}