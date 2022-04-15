using System.Text;

namespace RG.EscapeRoomProtocol
{

    public class ByteUtil
    {
        public static string ByteArrayToString(byte[] ba, int offset, int length)
        {
            StringBuilder hex = new StringBuilder(ba.Length * 2);
            for (int i = offset; i < offset + length; i++)
            {
                hex.AppendFormat("{0:x2}", ba[i]);
            }

            return hex.ToString();
        }

    }
}