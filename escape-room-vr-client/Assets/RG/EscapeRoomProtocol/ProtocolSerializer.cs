using System;
using System.Text;
using System.Threading.Tasks;
using RG.EscapeRoomProtocol.Messages;
using RG.EscapeRoom.Model.Math;

namespace RG.EscapeRoomProtocol
{
    public class ProtocolSerializer
    {

        private ByteFifoBuffer scratchByteFifoBuffer = new ByteFifoBuffer(1024);
        private byte[] floatScratchBuffer = new byte[4];
        private readonly PrimitiveSerializer primitiveSerializer;

        public ProtocolSerializer(PrimitiveSerializer primitiveSerializer)
        {
            this.primitiveSerializer = primitiveSerializer;
        }

        public int SerializeMessage(ClientConnectMessage message, ByteFifoBuffer byteFifoBuffer)
        {
            int size = 0;
            lock (scratchByteFifoBuffer)
            {
                size += primitiveSerializer.Write(MessageIds.ClientConnectMessage, scratchByteFifoBuffer);
                size += primitiveSerializer.Write(message.accessToken, scratchByteFifoBuffer);
                size += primitiveSerializer.Write((ushort) size, byteFifoBuffer);
                byteFifoBuffer.WriteAll(scratchByteFifoBuffer);
            }

            return size;
        }
        public int SerializeMessage(ClientWelcomeMessage message, ByteFifoBuffer byteFifoBuffer)
        {
            int size = 0;
            lock (scratchByteFifoBuffer)
            {
                size += primitiveSerializer.Write(MessageIds.ClientConnectMessage, scratchByteFifoBuffer);
                size += primitiveSerializer.Write(message.playerNetworkId, scratchByteFifoBuffer);
                size += primitiveSerializer.Write((ushort) size, byteFifoBuffer);
                byteFifoBuffer.WriteAll(scratchByteFifoBuffer);
            }

            return size;
        }

        public int SerializeMessage(LoadRoomMessage message, ByteFifoBuffer byteFifoBuffer)
        {
            int size = 0;
            lock (scratchByteFifoBuffer)
            {
                size += primitiveSerializer.Write(MessageIds.LoadRoomMessage, scratchByteFifoBuffer);
                size += primitiveSerializer.Write(message.roomDefinitionId, scratchByteFifoBuffer);
                size += primitiveSerializer.Write((ushort) size, byteFifoBuffer);
                byteFifoBuffer.WriteAll(scratchByteFifoBuffer);
            }

            return size;
   
        }

        public int SerializeMessage(RequestGrabMessage message, ByteFifoBuffer byteFifoBuffer)
        {
            int size = 0;
            lock (scratchByteFifoBuffer)
            {
                size += primitiveSerializer.Write(MessageIds.RequestGrabMessage, scratchByteFifoBuffer);
                size += SerializeGrabMessageContent(message, scratchByteFifoBuffer);
                size += primitiveSerializer.Write((ushort) size, byteFifoBuffer);
                byteFifoBuffer.WriteAll(scratchByteFifoBuffer);
            }

            return size;
        }

        public int SerializeMessage(GrabResultMessage message, ByteFifoBuffer byteFifoBuffer)
        {
            int size = 0;
            lock (scratchByteFifoBuffer)
            {
                size += primitiveSerializer.Write(MessageIds.GrabResultMessage, scratchByteFifoBuffer);
                size += SerializeGrabMessageContent(message.requestMessage, scratchByteFifoBuffer);
                size += primitiveSerializer.Write(message.wasSuccessful, scratchByteFifoBuffer);
                size += primitiveSerializer.Write((ushort) size, byteFifoBuffer);
                byteFifoBuffer.WriteAll(scratchByteFifoBuffer);
            }

            return size;
        }

        public int SerializeMessage(PlayerPositionMessage message, ByteFifoBuffer byteFifoBuffer)
        {
            int size = 0;
            lock (scratchByteFifoBuffer)
            {
                size += primitiveSerializer.Write(MessageIds.PlayerPositionMessage, scratchByteFifoBuffer);
                size += primitiveSerializer.Write(message.playerMessageBase, scratchByteFifoBuffer);
                size += primitiveSerializer.Write(message.headPosition, scratchByteFifoBuffer);
                size += primitiveSerializer.Write(message.headRotation, scratchByteFifoBuffer);
                size += primitiveSerializer.Write(message.leftHandPosition, scratchByteFifoBuffer);
                size += primitiveSerializer.Write(message.leftHandRotation, scratchByteFifoBuffer);
                size += primitiveSerializer.Write(message.rightHandPosition, scratchByteFifoBuffer);
                size += primitiveSerializer.Write(message.rightHandRotation, scratchByteFifoBuffer);
                
                size += primitiveSerializer.Write((ushort) size, byteFifoBuffer);
                byteFifoBuffer.WriteAll(scratchByteFifoBuffer);
            }

            return size;
        }

        private int SerializeGrabMessageContent(RequestGrabMessage message, ByteFifoBuffer byteFifoBuffer)
        {
            var size = primitiveSerializer.Write(message.playerMessageBase, scratchByteFifoBuffer);
            size += primitiveSerializer.Write(message.hand, scratchByteFifoBuffer);
            size += primitiveSerializer.Write(message.grabbableId, scratchByteFifoBuffer);
            size += primitiveSerializer.Write(message.isGrab, scratchByteFifoBuffer);
            return size;
        }

        public async Task DeserializeNextMessage(ByteFifoBuffer byteFifoBuffer, MessageReceiver receiver)
        {
            ushort nextMessageLength = await ReadShort(byteFifoBuffer);
            ushort nextMessageType = await ReadShort(byteFifoBuffer);
            switch (nextMessageType) 
            {
                case MessageIds.ClientConnectMessage:
                    await DeserializeHelloMessage(byteFifoBuffer, receiver);
                    break;
                case MessageIds.ClientWelcomeMessage:
                    await DeserializeClientWelcomeMessage(byteFifoBuffer, receiver);
                    break;
                case MessageIds.LoadRoomMessage: 
                    await DeserializeLoadRoomMessage(byteFifoBuffer, receiver);
                    break;
                case MessageIds.RequestGrabMessage: 
                    await DeserializeRequestGrabMessage(byteFifoBuffer, receiver);
                    break;
                case MessageIds.GrabResultMessage: 
                    await DeserializeGrabResultMessage(byteFifoBuffer, receiver);
                    break;
                case MessageIds.PlayerPositionMessage: 
                    await DeserializePlayerPositionMessage(byteFifoBuffer, receiver);
                    break;
                default:
                    await DiscardMessage(byteFifoBuffer, nextMessageType, nextMessageLength, receiver
                    );
                    break;
            }
        }

        private async Task DeserializePlayerPositionMessage(ByteFifoBuffer byteFifoBuffer, MessageReceiver receiver)
        {
                
            PlayerMessageBase playerMessageBase = await ReadMessageBase(byteFifoBuffer);
            Vector3 headPosition = await ReadVector3(byteFifoBuffer);
            Quaternion headRotation = await ReadQuaternion(byteFifoBuffer);
            Vector3 leftHandPosition = await ReadVector3(byteFifoBuffer);
            Quaternion leftHandRotation = await ReadQuaternion(byteFifoBuffer);
            Vector3 rightHandPosition = await ReadVector3(byteFifoBuffer);
            Quaternion rightHandRotation = await ReadQuaternion(byteFifoBuffer);
            var message = new PlayerPositionMessage(playerMessageBase,
                headPosition, headRotation,
                leftHandPosition, leftHandRotation,
                rightHandPosition, rightHandRotation);
            
            receiver.Receive(message);
        }

        private async Task DiscardMessage(ByteFifoBuffer byteFifoBuffer, ushort nextMessageType,
            ushort totalMessageLength,
            MessageReceiver messageReceiver)
        {
            var length = totalMessageLength - 2;
            await UntilByteFifoBufferContains(byteFifoBuffer, length);
            var array = new byte[length];
            byteFifoBuffer.Read(array,0,  length);
            messageReceiver.MessageDiscarded(nextMessageType);
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


        private async Task<Vector3> ReadVector3(ByteFifoBuffer byteFifoBuffer)
        {
            await UntilByteFifoBufferContains(byteFifoBuffer, 12);
            var x = ReadFloat(byteFifoBuffer);
            var y = ReadFloat(byteFifoBuffer);
            var z = ReadFloat(byteFifoBuffer);
            return new Vector3(x, y, z);
        }

        private float ReadFloat(ByteFifoBuffer byteFifoBuffer)
        {
            byteFifoBuffer.Read(floatScratchBuffer, 0, 4);
            return BitConverter.ToSingle(floatScratchBuffer, 0);
        }

        private async Task<Quaternion> ReadQuaternion(ByteFifoBuffer byteFifoBuffer)
        {
            
            await UntilByteFifoBufferContains(byteFifoBuffer, 16);
            var x = ReadFloat(byteFifoBuffer);
            var y = ReadFloat(byteFifoBuffer);
            var z = ReadFloat(byteFifoBuffer);
            var w = ReadFloat(byteFifoBuffer);
            return new Quaternion(x, y, z, w);
        }

        private async Task UntilByteFifoBufferContains(ByteFifoBuffer byteFifoBuffer, int leastNumberOfExpectedBytes)
        {
            while (byteFifoBuffer.Length < leastNumberOfExpectedBytes)
            {
                await Task.Yield();
            }
        }


    }

    public class PrimitiveSerializer
    {
        
        public int Write(PlayerMessageBase playerMessageBase, ByteFifoBuffer byteFifoBuffer)
        {
            int size = Write(playerMessageBase.senderId, byteFifoBuffer);
            return size;
        }
        
        public int Write(int i, ByteFifoBuffer byteFifoBuffer)
        {
            byteFifoBuffer.WriteByte( (byte) ((i>>24) & 0xff));
            byteFifoBuffer.WriteByte( (byte) ((i>>16) & 0xff));
            byteFifoBuffer.WriteByte( (byte) ((i>>8) & 0xff));
            byteFifoBuffer.WriteByte( (byte) ((i>>0) & 0xff));
            return 4;
        }
        
        public int Write(bool b, ByteFifoBuffer byteFifoBuffer)
        {
            byte byteValue = (byte) (b?1:0);
            byteFifoBuffer.WriteByte(byteValue);
            return 1;
        }
        
        public int Write(byte b, ByteFifoBuffer byteFifoBuffer)
        {
            byteFifoBuffer.WriteByte(b);
            return 1;
        }
        
        public int Write(ushort s, ByteFifoBuffer byteFifoBuffer)
        {
            var hiByte = (byte) ((s>>8) & 0xff);
            var loByte = (byte) (s & 0xff);
            byteFifoBuffer.WriteByte(hiByte);
            byteFifoBuffer.WriteByte(loByte);
            return 2;
        }
        public int Write(byte[] array, ByteFifoBuffer byteFifoBuffer)
        {
            var arrayLength = array.Length;
            var size = arrayLength;
            size += Write((ushort) arrayLength, byteFifoBuffer);
            byteFifoBuffer.Write(array,0,arrayLength);
            return size;
        }

        public int Write(Vector3 v, ByteFifoBuffer byteFifoBuffer)
        {
            var size = 0;
            size += Write(v.x, byteFifoBuffer);
            size += Write(v.y, byteFifoBuffer);
            size += Write(v.z, byteFifoBuffer);
            return size;
        }

        public int Write(Quaternion q, ByteFifoBuffer byteFifoBuffer)
        {
            var size = 0;
            size += Write(q.x, byteFifoBuffer);
            size += Write(q.y, byteFifoBuffer);
            size += Write(q.z, byteFifoBuffer);
            size += Write(q.w, byteFifoBuffer);
            return size;
        }

        public int Write(float f, ByteFifoBuffer byteFifoBuffer)
        {
            
            byteFifoBuffer.Write(BitConverter.GetBytes(f),0,4);
            return 4;
        }

        public int Write(string s, ByteFifoBuffer byteFifoBuffer)
        {
            return Write(Encoding.UTF8.GetBytes(s), byteFifoBuffer);
        }
    }
}