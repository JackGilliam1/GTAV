using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Methods;

namespace GTA.V.Formats.Save.Structs
{
    public class Entry
    {
        public string Name { get; set; }
        public Int32 DataLen { get; set; }
        public Stream CustomStream { get; set; }

        //Not actual properties
        private Stream xMain;
        public Int32 DataOffset { get; set; }
        public Entry(Stream xIn)
        {

            xMain = xIn;
            byte[] nameBuf = StreamUtils.ReadBytes(xMain, 4); //Reading 4 char name
            Name = Encoding.ASCII.GetString(nameBuf);
            if (Name == "pppp"){
                Name = null;
                return;
            }
            
            DataLen = StreamUtils.ReadInt32(xMain, true); //Reading data length
            DataOffset = (int)xMain.Position;

            xMain.Position -= 8;
            xMain.Position += (DataLen); //Advancing in position (blocksize - 8);
        }
        public void Write(Stream xOut)
        {
           
            byte[] nameBuf = Encoding.GetEncoding(1252).GetBytes(Name);
            xOut.Write(nameBuf, 0, nameBuf.Length);

            if (CustomStream == null)
                xMain.Position = DataOffset;
            else
                CustomStream.Position = 0;

            if (Name == "CHKS")
            {
                StreamUtils.WriteInt32(xOut, 20, true);
                StreamUtils.WriteInt32(xOut, 0, true);
                StreamUtils.WriteInt32(xOut, 0, true);
                StreamUtils.WriteBytes(xOut, new byte[4] { 0x78, 0x70, 0x70, 0x70 });
                

                long tPos = xOut.Position;
                xOut.Position = 0x0;
                byte[] inBuf = StreamUtils.ReadBytes(xOut, (int)xOut.Length);
                uint hash = GetHash(inBuf);
                xOut.Position = tPos;

                xOut.Position -= 12;
                StreamUtils.WriteInt32(xOut, (int)xOut.Length, true);
                StreamUtils.WriteUInt32(xOut, hash, true);
                xOut.Position += 4;
            }
            else
            {
                StreamUtils.WriteInt32(xOut, CustomStream == null ? DataLen : (int)CustomStream.Length + 8, true);
                StreamUtils.ReadBufferedStream(CustomStream == null ? xMain : CustomStream, CustomStream == null ? DataLen - 8 : (int)CustomStream.Length, xOut);
            }
        }
        public static uint GetHash(byte[] buffer)
        {
            uint hash = 0x3FAC7125;
            for (int i = 0; i < buffer.Length; i++)
            {

                hash += (uint)((sbyte)buffer[i]);
                hash += hash << 10;
                hash ^= hash >> 6;
            }
            hash += hash << 3;
            hash ^= hash >> 11;
            hash += hash << 15;
            return hash;
        }

        public void ExtractToStream(Stream xOut)
        {
            if (CustomStream == null)
            {
                xMain.Position = DataOffset;
                StreamUtils.ReadBufferedStream(xMain, DataLen- 8, xOut);
            }
            else
            {
                CustomStream.Position = 0;
                StreamUtils.ReadBufferedStream(CustomStream, (int)CustomStream.Length, xOut);
            }
        }
    }
}