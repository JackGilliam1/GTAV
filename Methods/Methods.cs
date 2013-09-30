using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.Compression;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

namespace Methods
{
    public class StreamUtils
    {
        #region Reading
        public static byte ReadByte(Stream xIn)
        {
            return (byte)xIn.ReadByte();
        }
        public static byte[] ReadBytes(Stream xIn, int len)
        {
            byte[] buffer = new byte[len];
            xIn.Read(buffer, 0, buffer.Length);
            return buffer;
        }
        public static Int16 ReadInt16(Stream xIn, bool rev)
        {
            byte[] buf = new byte[2];
            xIn.Read(buf, 0, buf.Length);
            if (rev)
                Array.Reverse(buf);
            return BitConverter.ToInt16(buf, 0);
        }
        public static Int32 ReadInt32(Stream xIn, bool rev)
        {
            byte[] buf = new byte[4];
            xIn.Read(buf, 0, buf.Length);
            if (rev)
                Array.Reverse(buf);
            return BitConverter.ToInt32(buf, 0);
        }
        public static Int64 ReadInt64(Stream xIn, bool rev)
        {
            byte[] buf = new byte[8];
            xIn.Read(buf, 0, buf.Length);
            if (rev)
                Array.Reverse(buf);
            return BitConverter.ToInt64(buf, 0);
        }
        public static char[] ReadNullChars(Stream xIn)
        {
            string xBuilt = string.Empty;
            while (true)
            {
                int read = xIn.ReadByte();
                if (read == 0)
                    break;
                xBuilt += Convert.ToChar(read);
            }
            return xBuilt.ToCharArray();
        }
        public static string ReadNullString(Stream xIn)
        {
            string xBuilt = string.Empty;
            while (true)
            {
                int read = xIn.ReadByte();
                if (read == 0)
                    break;
                xBuilt += Convert.ToChar(read);
            }
            return xBuilt;
        }
        #endregion

        #region Writing
        public static void WriteByte(Stream xOut, byte val)
        {
            xOut.Write(new byte[1] { val }, 0, 1);
        }
        public static void WriteBytes(Stream xOut, byte[] buffer)
        {
            xOut.Write(buffer, 0, buffer.Length);
        }
        public static void WriteInt16(Stream xOut, Int16 val, bool rev)
        {
            byte[] buffer = BitConverter.GetBytes(val);
            if (rev)
                Array.Reverse(buffer);
            xOut.Write(buffer, 0, buffer.Length);
        }
        public static void WriteInt32(Stream xOut, Int32 val, bool rev)
        {
            byte[] buffer = BitConverter.GetBytes(val);
            if (rev)
                Array.Reverse(buffer);
            xOut.Write(buffer, 0, buffer.Length);
        }
        public static void WriteInt64(Stream xOut, Int64 val, bool rev)
        {
            byte[] buffer = BitConverter.GetBytes(val);
            if (rev)
                Array.Reverse(buffer);
            xOut.Write(buffer, 0, buffer.Length);
        }
        public static void WriteUInt32(Stream xOut, UInt32 val, bool rev)
        {
            byte[] buffer = BitConverter.GetBytes(val);
            if (rev)
                Array.Reverse(buffer);
            xOut.Write(buffer, 0, buffer.Length);
        }
        public static void WriteNullChars(Stream xOut, char[] chars)
        {
            byte[] buffer = new byte[chars.Length];
            for (int i = 0; i < buffer.Length; i++)
                buffer[i] = Convert.ToByte(chars[i]);
            xOut.Write(buffer, 0, buffer.Length);
            xOut.Write(new byte[1] { 0x0 }, 0, 1);
        }
        public static void WriteNullString(Stream xOut, string str)
        {
            byte[] buffer = new byte[str.Length];
            for (int i = 0; i < buffer.Length; i++)
                buffer[i] = Convert.ToByte(str[i]);
            xOut.Write(buffer, 0, buffer.Length);
            xOut.Write(new byte[1] { 0x0 }, 0, 1);
        }
        public static void WriteUnicode(Stream xOut, string str)
        {
            byte[] buffer = Encoding.BigEndianUnicode.GetBytes(str);
            xOut.Write(buffer, 0, buffer.Length);
        }
        #endregion

        #region Other

        /// <summary>
        ///Read
        /// </summary>
        /// <param name="xMain">Main stream to read out of</param>
        /// <param name="readLen">Length to read</param>
        /// <param name="xOut">Stream to read ito</param>
        public static void ReadBufferedStream(Stream xMain, int readLen, Stream xOut)
        {
            int toOffset = (int)xMain.Position + readLen;
            while (xMain.Position < toOffset)
            {
                int rLength = ((toOffset - xMain.Position) >= 65536) ? 65536 : (toOffset - (int)xMain.Position);
                byte[] buffer = new byte[rLength];
                xMain.Read(buffer, 0, buffer.Length);
                xOut.Write(buffer, 0, buffer.Length);
            }
        }
        public static void CopyStream(Stream input, Stream output)
        {
            byte[] buffer = new byte[32768];
            int read;
            while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
            {
                output.Write(buffer, 0, read);
            }
        }
        public static byte[] Decompress(byte[] data)
        {
            var compressedStream = new MemoryStream(data);
            var zipStream = new DeflateStream(compressedStream, CompressionMode.Decompress);

            var resultStream = new MemoryStream();

            var buffer = new byte[4096];
            int read;

            while ((read = zipStream.Read(buffer, 0, buffer.Length)) > 0)
                resultStream.Write(buffer, 0, read);

            return resultStream.ToArray();
        }
        public static byte[] Compress(byte[] data)
        {
            var compressedStream = new MemoryStream();
            var zipStream = new DeflateStream(compressedStream, CompressionMode.Compress);
            zipStream.Write(data, 0, data.Length);
            return compressedStream.ToArray();
        }
        #endregion
    }
    public class CompressionUtils
    {
        [DllImport("xcompress32.dll")]
        public static extern int XMemCompress(int Context, byte[] pDestination, ref int pDestSize, byte[] pSource, int pSrcSize);
        [DllImport("xcompress32.dll")]
        public static extern int XMemCreateCompressionContext(XMEMCODEC_TYPE CodecType, int pCodecParams, int Flags, ref int pContext);
        [DllImport("xcompress32.dll")]
        public static extern int XMemResetCompressionContext(int Context);
        [DllImport("xcompress32.dll")]
        public static extern void XMemDestroyCompressionContext(int Context);
        public static byte[] Compress(byte[] dataToCompress)
        {
            int pContext = 0;
            int num2 = XMemCreateCompressionContext(XMEMCODEC_TYPE.XMEMCODEC_DEFAULT, 0, 0, ref pContext);
            int pDestSize = dataToCompress.Length * 2;
            byte[] pDestination = new byte[pDestSize];
            int length = dataToCompress.Length;
            num2 = XMemCompress(pContext, pDestination, ref pDestSize, dataToCompress, length);
            XMemDestroyCompressionContext(pContext);
            pDestSize -= 5;
            Array.Resize<byte>(ref pDestination, pDestSize);
            return pDestination;
        }

 
        public static int Decompress(byte[] compressedData, byte[] decompressedData)
        {
            int context = 0;
            int num2 = XMemCreateDecompressionContext(XMEMCODEC_TYPE.XMEMCODEC_LZX, 0, 0, ref context);
            int len = compressedData.Length;
            int destSize = decompressedData.Length;
            try
            {
                num2 = XMemDecompress(context, decompressedData, ref destSize, compressedData, len);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            return num2;
        }
        [DllImport("xcompress32.dll")]
        public static extern int XMemDecompress(int Context, byte[] pDestination, ref int pDestSize, byte[] pSource, int pSrcSize);
        [DllImport("xcompress32.dll")]
        public static extern void XMemDestroyDecompressionContext(int Context);
        [DllImport("xcompress32.dll")]
        public static extern int XMemResetDecompressionContext(int Context);
        [DllImport("xcompress32.dll")]
        public static extern int XMemCreateDecompressionContext(XMEMCODEC_TYPE CodecType, int pCodecParams, int Flags, ref int pContext);
        public enum XMEMCODEC_TYPE
        {
            XMEMCODEC_DEFAULT,
            XMEMCODEC_LZX
        }
    }
    public class StringUtils
    {
        public static byte[] ByteArrFromString(string str)
        {
            byte[] arr = new byte[str.Length / 2];
            int j = 0;
            for (int i = 0; i < str.Length; i += 2)
            {
                string hex = str.Substring(i, 2);
                arr[j] = (byte)ParseIntHex(hex);
                j++;
            }
            return arr;
        }
        public static int ParseIntHex(string hex)
        {
            if (hex == "" || hex == null)
                return 0;
            if (hex.StartsWith("0x"))
                hex = hex.Replace("0x", "");
            return int.Parse(hex, System.Globalization.NumberStyles.HexNumber);
        }
    }
    
}