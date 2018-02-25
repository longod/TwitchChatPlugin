using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Yukarinette;

namespace TwitchChatPlugin {
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
                if ( enableProxy ) {
                    if ( string.IsNullOrWhiteSpace( proxyIP ) ) {
                        return null;
                    }
                    return proxyIP;
                }
                return null;
            }
        }
        internal int? Port {
            get {
                if ( enableProxy ) {
                    int p = -1;
                    if ( int.TryParse( proxyPort, out p ) ) {
                        if ( p >= 0 ) {
                            return p;
                        }
                    }
                }
                return null;
            }
        }

        public string username = "";
        public string oauth = ""; // encrypted
        public string latency = "10.0"; // 指数とか浮動小数点の面倒な部分が出ないように文字列で保持
        public bool enableProxy = false;
        public string proxyIP = "";
        public string proxyPort = "";
    }
}
