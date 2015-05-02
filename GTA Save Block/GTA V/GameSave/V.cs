using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Methods;
using GTA.V.Formats.Save.Structs;
using GTA.V.Utils.Security;
namespace GTA.V.Formats.Save
{
    public class V
    {
        public Body Body { get; set; }
        private byte[] key;
        /// <summary>
        /// New GTA V GameSave
        /// </summary>
        /// <param name="xIn">Gamesave stream</param>
        /// <param name="key">Encryption key</param>
        public V(Stream xIn, byte[] key, bool dec)
        {
            this.key = key;
            MemoryStream xMem;
            byte[] xInBuf = StreamUtils.ReadBytes(xIn, (int)xIn.Length);
            if (dec)
            {
                byte[] deced = AESCryptor.Decrypt(xInBuf, key);
                xMem = new MemoryStream(deced);
            }
            else
                xMem = new MemoryStream(xInBuf);
            this.Body = new Body(xMem);
        }
        public void Write(Stream xOut, bool enc)
        {
            this.Body.Write(xOut);
            if (xOut.Length % 16 != 0)
                while (xOut.Length % 16 != 0)
                    xOut.WriteByte(0x70);
            byte[] xoutBuf = new byte[xOut.Length];
            xOut.Position = 0;
            xOut.Read(xoutBuf, 0, xoutBuf.Length);
            if (enc)
            {
                MemoryStream xMem = new MemoryStream(AESCryptor.Encrypt(xoutBuf, this.key));
                xOut.SetLength(0);
                xOut.Write(xMem.ToArray(), 0, (int)xMem.Length);
            }
            else
            {
                xOut.SetLength(0);
                xOut.Write(xoutBuf, 0,xoutBuf.Length);
            }

        }
        public Entry GetEntryByName(string name)
        {
            for (int i = 0; i < Body.Entries.Count; i++)
                if (Body.Entries[i].Name == name)
                    return Body.Entries[i];
            return null;
        }
    }
}