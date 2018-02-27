using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace TwitchChatPlugin {
    /// <summary>
    /// Encrypt / Decrypt
    /// </summary>
    internal static class Cryptography {

        internal static string Encrypt( string input ) {
            string passphrase = @"Eg2%p!SByvaM-ZpnXWa[6Ly)y%}#]4a\AcWc4}[)9&*#Z%ZraD8::h\MVLF=2F5>";
            return Encrypt( input, passphrase );
        }
        internal static string Decrypt( string input ) {
            string passphrase = @"Eg2%p!SByvaM-ZpnXWa[6Ly)y%}#]4a\AcWc4}[)9&*#Z%ZraD8::h\MVLF=2F5>";
            return Decrypt( input, passphrase );
        }

        internal static string Encrypt( string plaintext, string passphrase ) {
            if ( string.IsNullOrEmpty( plaintext ) || string.IsNullOrEmpty( passphrase ) ) {
                return null;
            }
            var bin = Encoding.UTF8.GetBytes( plaintext );
            var encrypted = Encrypt( bin, passphrase );
            return Convert.ToBase64String( encrypted );
        }

        internal static string Decrypt( string base64, string passphrase ) {
            if ( string.IsNullOrEmpty( base64 ) || string.IsNullOrEmpty( passphrase ) ) {
                return null;
            }
            var bin = Convert.FromBase64String( base64 );
            var decrypted = Decrypt( bin, passphrase );
            return Encoding.UTF8.GetString( decrypted );
        }

        static byte[] Encrypt( byte[] bin, string passphrase ) {
            if ( bin == null || bin.Length == 0 ) {
                return null;
            }
            if ( string.IsNullOrEmpty( passphrase ) ) {
                return null;
            }
            byte[] salt = GenerateRandom( keyBytes );
            byte[] iv = GenerateRandom( keyBytes );
            using ( var password = new Rfc2898DeriveBytes( passphrase, salt, derivationIterations ) ) {
                byte[] keyBytes = password.GetBytes( Cryptography.keyBytes ); 
                using ( var symmetricKey = new RijndaelManaged() ) {
                    symmetricKey.KeySize = keyBits;
                    symmetricKey.BlockSize = blockBits;
                    symmetricKey.Mode = mode;
                    symmetricKey.Padding = padding;
                    using ( var encryptor = symmetricKey.CreateEncryptor( keyBytes, iv ) ) {
                        using ( var ms = new MemoryStream() ) {
                            ms.Write( salt, 0, salt.Length );
                            ms.Write( iv, 0, iv.Length );
                            using ( var cryptoStream = new CryptoStream( ms, encryptor, CryptoStreamMode.Write ) ) {
                                cryptoStream.Write( bin, 0, bin.Length );
                                cryptoStream.FlushFinalBlock();
                                return ms.ToArray();
                            }
                        }
                    }
                }
            }
        }

        static byte[] Decrypt( byte[] bin, string passphrase ) {
            if ( bin == null || bin.Length < keyBytes * 2 ) {
                return null;
            }
            if ( string.IsNullOrEmpty( passphrase ) ) {
                return null;
            }
            using ( var ms = new MemoryStream( bin ) ) {
                byte[] salt = new byte[ keyBytes ];
                ms.Read( salt, 0, keyBytes );
                byte[] iv = new byte[ keyBytes ];
                ms.Read( iv, 0, keyBytes );
                using ( var password = new Rfc2898DeriveBytes( passphrase, salt, derivationIterations ) ) {
                    var keyBytes = password.GetBytes( Cryptography.keyBytes );
                    using ( var symmetricKey = new RijndaelManaged() ) {
                        symmetricKey.KeySize = keyBits;
                        symmetricKey.BlockSize = blockBits;
                        symmetricKey.Mode = mode;
                        symmetricKey.Padding = padding;
                        using ( var decryptor = symmetricKey.CreateDecryptor( keyBytes, iv ) ) {
                            using ( var cryptoStream = new CryptoStream( ms, decryptor, CryptoStreamMode.Read ) ) {
                                using ( var os = new MemoryStream() ) {
                                    byte[] temp = new byte[ 4096 ];
                                    int len = 0;
                                    while ( ( len = cryptoStream.Read( temp, 0, temp.Length ) ) > 0 ) {
                                        os.Write( temp, 0, len );
                                    }
                                    return os.ToArray();
                                }
                            }
                        }
                    }
                }
            }
        }

        static byte[] GenerateRandom( int size ) {
            using ( var rngCsp = new RNGCryptoServiceProvider() ) {
                byte[] randomBytes = new byte[ size ];
                rngCsp.GetBytes( randomBytes );
                return randomBytes;
            }
        }

        static readonly CipherMode mode = CipherMode.CBC;
        static readonly PaddingMode padding = PaddingMode.PKCS7;
        static readonly int derivationIterations = 1000;
        static readonly int keyBits = 256; // AES=128, Rijndael=256
        static readonly int blockBits = 256;
        static readonly int keyBytes = keyBits / 8; // 32 Bytes will give us 256 bits.
    }
}
