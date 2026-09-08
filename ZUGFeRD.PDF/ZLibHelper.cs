using System.IO;
using System.IO.Compression;

namespace s2industries.ZUGFeRD.PDF
{
    public class ZLibHelper
    {
        public static byte[] CompressWithBestCompression(byte[] rawBytes)
        {
            uint adler32 = ComputeAdler32(rawBytes);

            using (var outputStream = new MemoryStream())
            {
                outputStream.WriteByte(0x78);
                outputStream.WriteByte(0xDA);

                using (var deflateStream = new DeflateStream(outputStream, CompressionLevel.Optimal, true))
                {
                    deflateStream.Write(rawBytes, 0, rawBytes.Length);
                }

                outputStream.WriteByte((byte)((adler32 >> 24) & 0xFF));
                outputStream.WriteByte((byte)((adler32 >> 16) & 0xFF));
                outputStream.WriteByte((byte)((adler32 >> 8) & 0xFF));
                outputStream.WriteByte((byte)(adler32 & 0xFF));

                return outputStream.ToArray();
            }
        }

        private static uint ComputeAdler32(byte[] data)
        {
            const uint BASE = 65521; 
            uint s1 = 1;
            uint s2 = 0;

            for (int i = 0; i < data.Length; i++)
            {
                s1 = (s1 + data[i]) % BASE;
                s2 = (s2 + s1) % BASE;
            }

            return (s2 << 16) | s1;
        }
    }
}
