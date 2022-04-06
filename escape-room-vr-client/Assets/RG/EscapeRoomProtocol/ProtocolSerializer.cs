using System;
using System.Collections.Generic;
using System.IO;
using RG.EscapeRoomProtocol.Messages;

namespace RG.EscapeRoomProtocol
{
    public class ProtocolSerializer
    {

        private static Dictionary<Type, ushort> messageIds = new Dictionary<Type, ushort>();
        private MemoryStream scratchStream = new MemoryStream(new byte[MAX_MESSAGE_SIZE]);
        private const int MAX_MESSAGE_SIZE = 1024;

        static ProtocolSerializer()
        {
            messageIds[typeof(ClientHelloMessage)] = 0;
        }

        public int SerializeMessage(ClientHelloMessage message, Stream stream)
        {
            int size = 0;
            lock (scratchStream)
            {
                size += Write(messageIds[message.GetType()], scratchStream);
                size += Write(message.accessToken, scratchStream);
                Write((ushort) size, stream);
                scratchStream.Flush();
                scratchStream.CopyTo(stream);
                scratchStream.SetLength(0);
            }

            return size;
        }

        private int Write(ushort s, Stream stream)
        {
            stream.WriteByte((byte) ((s>>8) & 0xff));
            stream.WriteByte((byte) (s & 0xff));
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
    }
}