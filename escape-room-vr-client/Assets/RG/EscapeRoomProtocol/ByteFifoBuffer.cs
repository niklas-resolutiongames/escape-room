namespace RG.EscapeRoomProtocol
{
    public class ByteFifoBuffer
    {
        private byte[] buffer;
        private int writePosition;
        private int readPosition;

        public ByteFifoBuffer(int capacity)
        {
            buffer = new byte[capacity];
        }

        public int Length
        {
            get
            {
                var l = writePosition - readPosition;
                while (l < 0)
                {
                    l += buffer.Length;
                }
                return l;
            }
        }

        public void Reset()
        {
            writePosition = 0;
            readPosition = 0;
        }

        public void Read(byte[] destinationArray, int offset, int length)
        {
            for (int i = 0; i < length; i++)
            {
                destinationArray[i + offset] = ReadByte();
            }
        }

        public byte ReadByte()
        {
            return buffer[GetAndIncrementReadPosition()];
        }

        public void WriteByte(byte b)
        {
            buffer[GetAndIncrementWritePosition()] = b;
        }


        private int GetAndIncrementReadPosition()
        {
            int position = readPosition;
            readPosition++;
            while (readPosition >= buffer.Length)
            {
                readPosition -= buffer.Length;
            }

            return position;
        }

        private int GetAndIncrementWritePosition()
        {
            int position = writePosition;
            writePosition++;
            while (writePosition >= buffer.Length)
            {
                writePosition -= buffer.Length;
            }

            return position;
        }
        public void Write(byte[] array, int offset, int length)
        {
            for (int i = 0; i < length; i++)
            {
                WriteByte(array[i + offset]);
            }
        }

        public void WriteAll(ByteFifoBuffer otherBuffer)
        {
            while (otherBuffer.Length > 0)
            {
                WriteByte(otherBuffer.ReadByte());
            }
        }

        public byte[] ReadAllAsArray()
        {
            int l = Length;
            var buffer = new byte[l];
            Read(buffer, 0, l);
            return buffer;
        }
    }
}