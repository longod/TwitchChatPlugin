using System.Windows;
using Yukarinette;

namespace TwitchChatPlugin {
    /// <summary>
    /// 
    /// </summary>
    public class Plugin : IYukarinetteInterface {
        public override string Name {
            get {
                return "Twitch Chat";
            }
        }

        public override string GUID => base.GUID; // 変えて欲しいのであれば、abstructにするべきでは…

        public override void Loaded() {
            settings = Settings.LoadFromSavedLocation();
        }

        public override void Closed() {
            client?.Disconnect();
            client = null; // dispose?
        }

        public override void SpeechRecognitionStart() {
            // recreate every time
            client = new ChatClient( settings.username, Cryptography.Decrypt( settings.oauth ), settings.Latency, settings.ProxyIP, settings.Port );
            client?.Connect();
        }

        public override void SpeechRecognitionStop() {
            client?.Disconnect();
            client = null; // dispose?
        }

        public override void Speech( string text ) {
            // どこかの段階で末尾についているが、日本語の喋りの慣習的に句読点はあまり重要ではないので除去する
            if ( text.EndsWith( "." ) ) {
                text = text.Remove( text.Length - 1 );
            }
            client?.EnqueueMessage( text );
        }

        public override void Setting() {
            var s = new SettingsWindow();
            if ( s.Show( Application.Current.MainWindow, settings ) ) {
                settings = s.Accept();
                Settings.SaveToSavedLocation( settings );
            }
        }

        ChatClient client = null;
        Settings settings = null;

    }
}
