using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ConsImport
{
    public class Utils
    {
        public static string ComputeHash(string input)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // ComputeHash - returns byte array  
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(input));
                // Convert byte array to a string   
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
        public static string Decrypt(string strEncrypted, string strKey)
        {
            if (string.IsNullOrWhiteSpace(strEncrypted)) return strEncrypted;
            try
            {
                TripleDESCryptoServiceProvider objDESCrypto =
                    new TripleDESCryptoServiceProvider();
                MD5CryptoServiceProvider objHashMD5 = new MD5CryptoServiceProvider();
                byte[] byteHash, byteBuff;
                string strTempKey = strKey;
                byteHash = objHashMD5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(strTempKey));
                objHashMD5 = null;
                objDESCrypto.Key = byteHash;
                objDESCrypto.Mode = CipherMode.ECB; //CBC, CFB
                byteBuff = Convert.FromBase64String(strEncrypted);
                string strDecrypted = ASCIIEncoding.ASCII.GetString
                (objDESCrypto.CreateDecryptor().TransformFinalBlock
                (byteBuff, 0, byteBuff.Length));
                objDESCrypto = null;
                return strDecrypted;
            }
            catch (Exception ex)
            {
                return "Błąd odczytu" + ex.Message;
            }
        }

        public static string SerializeToXmlString<T>(T obj)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            using (StringWriter writer = new StringWriter())
            {
                serializer.Serialize(writer, obj);
                return writer.ToString();
            }
        }
        public static T DeserializeFromXmlString<T>(string xml)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            using (StringReader reader = new StringReader(xml))
            {
                return (T)serializer.Deserialize(reader);
            }
        }

        internal static byte[] CreateZipArchive(IEnumerable<KeyValuePair<string, byte[]>> documents)
        {
            using (var outputStream = new MemoryStream())
            {
                using (var zipStream = new ZipOutputStream(outputStream) { IsStreamOwner = false })
                {
                    var usedNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                    foreach (var document in documents)
                    {
                        string entryName = document.Key;
                        string nameWithoutExtension = Path.GetFileNameWithoutExtension(entryName);
                        string extension = Path.GetExtension(entryName);
                        int suffix = 2;
                        while (!usedNames.Add(entryName))
                        {
                            entryName = nameWithoutExtension + "_" + suffix++ + extension;
                        }

                        zipStream.PutNextEntry(new ZipEntry(entryName)
                        {
                            IsUnicodeText = true,
                            Size = document.Value.Length
                        });
                        zipStream.Write(document.Value, 0, document.Value.Length);
                        zipStream.CloseEntry();
                    }
                }

                return outputStream.ToArray();
            }
        }

        public static byte[] DecompressMsWord(byte[] input)
        {
            if (input == null || input.Length == 0)
                return input;

            int offset = 0;

            // Format: 4 bajty długości + zlib, zwykle 78 9C
            if (input.Length > 6 && input[4] == 0x78)
                offset = 4;

            using (var inputStream = new MemoryStream(input, offset, input.Length - offset))
            using (var inflater = new InflaterInputStream(inputStream))
            using (var outputStream = new MemoryStream())
            {
                inflater.CopyTo(outputStream);
                return outputStream.ToArray();
            }
        }
    }
}
