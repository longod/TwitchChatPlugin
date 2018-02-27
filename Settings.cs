using Newtonsoft.Json;
using System.IO;
using Yukarinette;

namespace TwitchChatPlugin {
    /// <summary>
    /// User configuration
    /// </summary>
    internal class Settings {
        static internal string Filename { get { return "TwitchChatPlugin.dll.config"; } }

        static internal string SavedLocation {
            get {
                string dir = Path.Combine( YukarinetteCommon.AppSettingFolder, "plugins" );
                string path = Path.Combine( dir, Filename );
                return path;
            }
        }

        static internal void SaveToSavedLocation( Settings settings ) {
            Save( SavedLocation, settings );
        }

        static internal Settings LoadFromSavedLocation() {
            return Load( SavedLocation );
        }

        static internal void Save( string path, Settings settings ) {
            string json = JsonConvert.SerializeObject( settings );
            File.WriteAllText( path, json );
        }

        static internal Settings Load( string path ) {
            if ( File.Exists( path ) ) {
                string text = File.ReadAllText( path );
                if ( string.IsNullOrEmpty( text ) == false ) {
                    Settings settings = JsonConvert.DeserializeObject<Settings>( text );
                    return settings;
                }
            }
            return new Settings();
        }

        internal double Latency {
            get {
                double l = 0.0;
                double.TryParse( latency, out l );
                return l;
            }
        }

        internal string ProxyIP {
            get {
                if ( enableProxy && string.IsNullOrWhiteSpace( proxyIP ) == false ) {
                    return proxyIP;
                }
                return null;
            }
        }

        internal int? Port {
            get {
                int p = -1;
                if ( enableProxy && int.TryParse( proxyPort, out p ) ) {
                    if ( p >= 0 ) {
                        return p;
                    }
                }
                return null;
            }
        }

        /// <summary>If you update format you must increment and keep compatibility.</summary>
        public int version = 0; // 
        /// <summary>Username (plain)</summary>
        public string username = "";
        /// <summary>OAuth token (encrypted)</summary>
        public string oauth = "";
        /// <summary>Sending latency seconds for streaming gap</summary>
        public string latency = "10.0"; // 指数とか浮動小数点の面倒な部分が出ないように文字列で保持
        /// <summary>Using proxy</summary>
        public bool enableProxy = false;
        /// <summary>Proxy host</summary>
        public string proxyIP = "";
        /// <summary> Proxy port</summary>
        public string proxyPort = "";
    }
}
