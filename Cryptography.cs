using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;


namespace TwitchChatPlugin {
    // https://www.codeproject.com/Articles/769741/Csharp-AES-bits-Encryption-Library-with-Salt
    static internal class Cryptography {
        static readonly byte[] saltBytes = { 0x74, 0x77, 0x69, 0x74, 0x63, 0x68, 0x74, 0x76 };
        static readonly int keySizes = 256;
        static readonly int blockSize = 128;

        static byte[] Encrypt( byte[] bytesToBeEncrypted, byte[] passwordBytes ) {
            if ( bytesToBeEncrypted == null || bytesToBeEncrypted.Length == 0 ) {
                return null;
            }
            if ( passwordBytes == null || passwordBytes.Length == 0 ) {
                return null;
            }
            byte[] encryptedBytes = null;
            using ( var ms = new MemoryStream() ) {
                using ( var m = new RijndaelManaged() ) {
                    m.KeySize = keySizes;
                    m.BlockSize = blockSize;
                    var key = new Rfc2898DeriveBytes( passwordBytes, saltBytes, 1000 );
                    m.Key = key.GetBytes( m.KeySize / 8 );
                    m.IV = key.GetBytes( m.BlockSize / 8 );
                    m.Mode = CipherMode.CBC;
                    using ( var cs = new CryptoStream( ms, m.CreateEncryptor(), CryptoStreamMode.Write ) ) {
                        cs.Write( bytesToBeEncrypted, 0, bytesToBeEncrypted.Length );
                        cs.Close();
                    }
                    encryptedBytes = ms.ToArray();
                }
            }
            return encryptedBytes;
        }
        static byte[] Decrypt( byte[] bytesToBeDecrypted, byte[] passwordBytes ) {
            if ( bytesToBeDecrypted == null || bytesToBeDecrypted.Length == 0 ) {
                return null;
            }
            if ( passwordBytes == null || passwordBytes.Length == 0 ) {
                return null;
            }
            byte[] decryptedBytes = null;
            using ( MemoryStream ms = new MemoryStream() ) {
                using ( RijndaelManaged m = new RijndaelManaged() ) {
                    m.KeySize = keySizes;
                    m.BlockSize = blockSize;
                    var key = new Rfc2898DeriveBytes( passwordBytes, saltBytes, 1000 );
                    m.Key = key.GetBytes( m.KeySize / 8 );
                    m.IV = key.GetBytes( m.BlockSize / 8 );
                    m.Mode = CipherMode.CBC;
                    using ( var cs = new CryptoStream( ms, m.CreateDecryptor(), CryptoStreamMode.Write ) ) {
                        cs.Write( bytesToBeDecrypted, 0, bytesToBeDecrypted.Length );
                        cs.Close();
                    }
                    decryptedBytes = ms.ToArray();
                }
            }
            return decryptedBytes;
        }

        static internal string Encrypt( string input, string password ) {
            if ( string.IsNullOrEmpty( input ) || string.IsNullOrWhiteSpace( password ) ) {
                return null;
            }
            byte[] bytesToBeEncrypted = Encoding.UTF8.GetBytes( input );
            byte[] passwordBytes = Encoding.UTF8.GetBytes( password );
            passwordBytes = SHA256.Create().ComputeHash( passwordBytes );
            byte[] bytesEncrypted = Encrypt( bytesToBeEncrypted, passwordBytes );
            string result = Convert.ToBase64String( bytesEncrypted );
            return result;
        }
        static internal string Decrypt( string input, string password ) {
            if ( string.IsNullOrEmpty( input ) || string.IsNullOrWhiteSpace( password ) ) {
                return null;
            }
            byte[] bytesToBeDecrypted = Convert.FromBase64String( input );
            byte[] passwordBytes = Encoding.UTF8.GetBytes( password );
            passwordBytes = SHA256.Create().ComputeHash( passwordBytes );
            byte[] bytesDecrypted = Decrypt( bytesToBeDecrypted, passwordBytes );
            string result = Encoding.UTF8.GetString( bytesDecrypted );
            return result;
        }

        static internal string Encrypt( string input ) {
            var attr = (System.Runtime.InteropServices.GuidAttribute)Attribute.GetCustomAttribute( System.Reflection.Assembly.GetExecutingAssembly(), typeof( System.Runtime.InteropServices.GuidAttribute ) );
            return Encrypt( input, attr.Value );
        }
        static internal string Decrypt( string input ) {
            var attr = (System.Runtime.InteropServices.GuidAttribute)Attribute.GetCustomAttribute( System.Reflection.Assembly.GetExecutingAssembly(), typeof( System.Runtime.InteropServices.GuidAttribute ) );
            return Decrypt( input, attr.Value );
        }
    }
}
