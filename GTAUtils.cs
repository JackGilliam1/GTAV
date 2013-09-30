using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using Methods;

namespace GTA.V.Utils.Security
{
    public static class AESCryptor
    {


        public static byte[] Decrypt(byte[] data, byte[] key)
        {
            RijndaelManaged aes = new RijndaelManaged();
            aes = new RijndaelManaged();
            aes.KeySize = 256;
            aes.BlockSize = 128;
            aes.Key = key;
            aes.Mode = CipherMode.ECB;
            aes.Padding = PaddingMode.Zeros;
            data = aes.CreateDecryptor().TransformFinalBlock(data, 0, data.Length);
            return data;
        }
        public static byte[] Encrypt(byte[] data, byte[] key)
        {
            RijndaelManaged aes = new RijndaelManaged();
            aes = new RijndaelManaged();
            aes.KeySize = 256;
            aes.BlockSize = 128;
            aes.Key = key;
            aes.Mode = CipherMode.ECB;
            aes.Padding = PaddingMode.Zeros;
            data = aes.CreateEncryptor().TransformFinalBlock(data, 0, data.Length);
            return data;
        }
   

    }
    public class KeyUtils
    {
        public static byte[] GetKey(string path)
        {
            if (File.Exists(path))
            {
                Stream xIn = File.Open(path, FileMode.Open);
               
                StreamReader xReader = new StreamReader(xIn);
                while (!xReader.EndOfStream)
                {
                  
                    string line = xReader.ReadLine();
                    if (line.Substring(0, 5) == "GTAV=")
                    {
                        string keyHex = line.Substring(5);
                        if (keyHex.Length / 2 != 32 || keyHex == "")
                        {
                            xReader.Close();
                            return null;
                        }

                        byte[] key = StringUtils.ByteArrFromString(keyHex);
                        xReader.Close();
                        return key;
                    }
                }
                xReader.Close();
                return null;
          
            }
            else
                return null;
            
        }
    }
}