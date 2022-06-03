using System.Text;
using System.Threading.Tasks;
using RG.EscapeRoomProtocol.Messages;

namespace RG.EscapeRoomProtocol
{
    public class ProtocolSerializer
    {

        private ByteFifoBuffer scratchByteFifoBuffer = new ByteFifoBuffer(1024);

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
        public int SerializeMessage(ClientWelcomeMessage message, ByteFifoBuffer byteFifoBuffer)
        {
            int size = 0;
            lock (scratchByteFifoBuffer)
            {
                size += Write(ClientConnectMessage.ID, scratchByteFifoBuffer);
                size += Write(message.playerNetworkId, scratchByteFifoBuffer);
                size += Write((ushort) size, byteFifoBuffer);
                byteFifoBuffer.WriteAll(scratchByteFifoBuffer);
            }

            return size;
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

        public int SerializeMessage(RequestGrabMessage message, ByteFifoBuffer byteFifoBuffer)
        {
            int size = 0;
            lock (scratchByteFifoBuffer)
            {
                size += Write(RequestGrabMessage.ID, scratchByteFifoBuffer);
                size += SerializeGrabMessageContent(message, scratchByteFifoBuffer);
                size += Write((ushort) size, byteFifoBuffer);
                byteFifoBuffer.WriteAll(scratchByteFifoBuffer);
            }

            return size;
        }

        public int SerializeMessage(GrabResultMessage message, ByteFifoBuffer byteFifoBuffer)
        {
            int size = 0;
            lock (scratchByteFifoBuffer)
            {
                size += Write(GrabResultMessage.ID, scratchByteFifoBuffer);
                size += SerializeGrabMessageContent(message.requestMessage, scratchByteFifoBuffer);
                size += Write(message.wasSuccessful, scratchByteFifoBuffer);
                size += Write((ushort) size, byteFifoBuffer);
                byteFifoBuffer.WriteAll(scratchByteFifoBuffer);
            }

            return size;
        }

        private int SerializeGrabMessageContent(RequestGrabMessage message, ByteFifoBuffer byteFifoBuffer)
        {
            var size = Write(message.playerMessageBase, scratchByteFifoBuffer);
            size += Write(message.hand, scratchByteFifoBuffer);
            size += Write(message.grabbableId, scratchByteFifoBuffer);
            size += Write(message.isGrab, scratchByteFifoBuffer);
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
                case ClientWelcomeMessage.ID:
                    await DeserializeClientWelcomeMessage(byteFifoBuffer, receiver);
                    break;
                case LoadRoomMessage.ID: 
                    await DeserializeLoadRoomMessage(byteFifoBuffer, receiver);
                    break;
                case RequestGrabMessage.ID: 
                    await DeserializeRequestGrabMessage(byteFifoBuffer, receiver);
                    break;
                case GrabResultMessage.ID: 
                    await DeserializeGrabResultMessage(byteFifoBuffer, receiver);
                    break;
            }
        }

        private async Task DeserializeGrabResultMessage(ByteFifoBuffer byteFifoBuffer, MessageReceiver receiver)
        {
            RequestGrabMessage requestMessage = await DeserializeRequestGrabContent(byteFifoBuffer);
            bool wasSuccessful = await ReadBool(byteFifoBuffer);
            receiver.Receive(new GrabResultMessage(requestMessage, wasSuccessful));
        }
        private async Task DeserializeRequestGrabMessage(ByteFifoBuffer byteFifoBuffer, MessageReceiver receiver)
        {
            var requestGrabMessage = await DeserializeRequestGrabContent(byteFifoBuffer);
            receiver.Receive(requestGrabMessage);
        }

        private async Task<RequestGrabMessage> DeserializeRequestGrabContent(ByteFifoBuffer byteFifoBuffer)
        {
            var playerMessageBase = await ReadMessageBase(byteFifoBuffer);
            var hand = await ReadByte(byteFifoBuffer);
            var grabbableId = await ReadString(byteFifoBuffer);
            var isGrab = await ReadBool(byteFifoBuffer);
            var requestGrabMessage = new RequestGrabMessage(playerMessageBase, hand, grabbableId, isGrab);
            return requestGrabMessage;
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


        private async Task DeserializeClientWelcomeMessage(ByteFifoBuffer byteFifoBuffer, MessageReceiver receiver)
        {
            var playerNetworkId = await ReadInt(byteFifoBuffer);
            var message = new ClientWelcomeMessage(playerNetworkId);
            receiver.Receive(message);
        }

        private async Task<PlayerMessageBase> ReadMessageBase(ByteFifoBuffer byteFifoBuffer)
        {
            var arr = await ReadInt(byteFifoBuffer);
            return new PlayerMessageBase(arr);
        }



        private async Task<byte[]> ReadArray(ByteFifoBuffer byteFifoBuffer)
        {
            var length = await ReadShort(byteFifoBuffer);
            await UntilByteFifoBufferContains(byteFifoBuffer, length);
            var array = new byte[length];
            byteFifoBuffer.Read(array,0,  length);
            return array;
        }
        
        private async Task<string> ReadString(ByteFifoBuffer byteFifoBuffer)
        {
            return Encoding.UTF8.GetString(await ReadArray(byteFifoBuffer));
        }

        private async Task<bool> ReadBool(ByteFifoBuffer byteFifoBuffer)
        {
            await UntilByteFifoBufferContains(byteFifoBuffer, 1);
            var b = (byte) byteFifoBuffer.ReadByte();
            return b == 1;
        }

        private async Task<byte> ReadByte(ByteFifoBuffer byteFifoBuffer)
        {
            await UntilByteFifoBufferContains(byteFifoBuffer, 1);
            var b = (byte) byteFifoBuffer.ReadByte();
            return b;
        }

        private async Task<ushort> ReadShort(ByteFifoBuffer byteFifoBuffer)
        {
            await UntilByteFifoBufferContains(byteFifoBuffer, 2);
            var hiByte = (byte) byteFifoBuffer.ReadByte();
            var loByte = (byte) byteFifoBuffer.ReadByte();
            return (ushort) ((hiByte << 8) + loByte);
        }

        private async Task<int> ReadInt(ByteFifoBuffer byteFifoBuffer)
        {
            await UntilByteFifoBufferContains(byteFifoBuffer, 4);
            int r = 0;
            for (int i = 0; i < 4; i++)
            {
                r <<= 8;
                r += byteFifoBuffer.ReadByte();
            }

            return r;
        }

        private async Task UntilByteFifoBufferContains(ByteFifoBuffer byteFifoBuffer, int leastNumberOfExpectedBytes)
        {
            while (byteFifoBuffer.Length < leastNumberOfExpectedBytes)
            {
                await Task.Yield();
            }
        }

        private int Write(PlayerMessageBase playerMessageBase, ByteFifoBuffer byteFifoBuffer)
        {
            int size = Write(playerMessageBase.senderId, byteFifoBuffer);
            return size;
        }
        
        private int Write(int i, ByteFifoBuffer byteFifoBuffer)
        {
            byteFifoBuffer.WriteByte( (byte) ((i>>24) & 0xff));
            byteFifoBuffer.WriteByte( (byte) ((i>>16) & 0xff));
            byteFifoBuffer.WriteByte( (byte) ((i>>8) & 0xff));
            byteFifoBuffer.WriteByte( (byte) ((i>>0) & 0xff));
            return 4;
        }
        
        private int Write(bool b, ByteFifoBuffer byteFifoBuffer)
        {
            byte byteValue = (byte) (b?1:0);
            byteFifoBuffer.WriteByte(byteValue);
            return 1;
        }
        
        private int Write(byte b, ByteFifoBuffer byteFifoBuffer)
        {
            byteFifoBuffer.WriteByte(b);
            return 1;
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