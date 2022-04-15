using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using RG.EscapeRoomProtocol.Messages;

namespace RG.EscapeRoomProtocol
{
    public class ProtocolSerializer
    {

        private MemoryStream scratchStream = new MemoryStream(buffer);
        private static byte[] buffer = new byte[MAX_MESSAGE_SIZE];
        private const int MAX_MESSAGE_SIZE = 1024;

        public int SerializeMessage(ClientConnectMessage message, Stream stream)
        {
            int size = 0;
            lock (scratchStream)
            {
                size += Write(ClientConnectMessage.ID, scratchStream);
                size += Write(message.accessToken, scratchStream);
                size += Write((ushort) size, stream);
                stream.Write(buffer, 0, size);
                scratchStream.SetLength(0);
            }

            return size;
        }

        public async Task DeserializeNextMessage(Stream stream, MessageReceiver receiver)
        {
            ushort nextMessageLength = await ReadShort(stream);
            ushort nextMessageType = await ReadShort(stream);
            switch (nextMessageType) 
            {
                case ClientConnectMessage.ID:
                    await DeserializeHelloMessage(stream, receiver);
                    break;
                case LoadRoomMessage.ID: 
                    await DeserializeLoadRoomMessage(stream, receiver);
                    break;
            }
        }

        private async Task DeserializeLoadRoomMessage(Stream stream, MessageReceiver receiver)
        {
            byte[] stringBytes = await ReadArray(stream);
            string roomId = Encoding.UTF8.GetString(stringBytes);
            var message = new LoadRoomMessage(roomId);
            receiver.Receive(message);
        }

        private async Task DeserializeHelloMessage(Stream stream, MessageReceiver receiver)
        {
            byte[] accessToken = await ReadArray(stream);
            var message = new ClientConnectMessage(accessToken);
            receiver.Receive(message);
        }
        
        
        public int SerializeMessage(LoadRoomMessage message, MemoryStream stream)
        {
            int size = 0;
            lock (scratchStream)
            {
                size += Write(LoadRoomMessage.ID, scratchStream);
                size += Write(message.roomDefinitionId, scratchStream);
                size += Write((ushort) size, stream);
                stream.Write(buffer, 0, size);
                scratchStream.SetLength(0);
            }

            return size;
   
        }

        private async Task<byte[]> ReadArray(Stream stream)
        {
            var length = await ReadShort(stream);
            await UntilStreamContains(stream, length);
            var array = new byte[length];
            stream.Read(array, 0, length);
            return array;
        }

        private async Task<ushort> ReadShort(Stream stream)
        {
            await UntilStreamContains(stream, 2);
            var hiByte = (byte) stream.ReadByte();
            var loByte = (byte) stream.ReadByte();
            return (ushort) ((hiByte << 8) + loByte);
        }

        private async Task UntilStreamContains(Stream stream, int leastNumberOfExpectedBytes)
        {
            while (stream.Length < leastNumberOfExpectedBytes)
            {
                await Task.Yield();
            }
        }

        private int Write(ushort s, Stream stream)
        {
            var hiByte = (byte) ((s>>8) & 0xff);
            var loByte = (byte) (s & 0xff);
            stream.WriteByte(hiByte);
            stream.WriteByte(loByte);
            return 2;
        }
        private int Write(byte[] array, Stream stream)
        {
            var arrayLength = array.Length;
            var size = arrayLength;
            size += Write((ushort) arrayLength, stream);
            stream.Write(array,0,arrayLength);
            return size;
        }
        private int Write(string s, Stream stream)
        {
            return Write(Encoding.UTF8.GetBytes(s), stream);
        }

    }
}