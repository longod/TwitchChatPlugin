using TwitchLib;
using TwitchLib.Events.Client;
using TwitchLib.Models.Client;
using Yukarinette; // wrap logger and exclude

namespace TwitchChatPlugin {
    /// <summary>
    /// Client of TwitchChat
    /// </summary>
    internal class ChatClient {
        internal ChatClient( string username, string oauth, double latency, string proxyIP = null, int? proxyPort = null, bool dryRun = false ) {
            crentials = new ConnectionCredentials(
                twitchUsername: username,
                twitchOAuth: oauth,
                proxyIP: proxyIP, 
                proxyPort: proxyPort );
            client = new TwitchClient( crentials, username, logging: false );  // channnel is same as username.
            client.OnConnected += Client_OnConnected;
            client.OnDisconnected += Client_OnDisconnected;
            client.OnLog += Client_OnLog;
            client.OnConnectionError += Client_OnConnectionError;
            this.latency = latency;
            this.dryRun = dryRun;
        }

        internal bool IsConnected {
            get {
                if ( client != null && client.IsConnected ) {
                    return true;
                }
                return false;
            }
        }

        internal void Connect() {
            if ( client != null && !IsConnected ) {
                YukarinetteLogger.Instance.Info( "connecting..." );
                client.Connect();
            }
        }

        internal void Disconnect() {
            if ( IsConnected ) {
                YukarinetteLogger.Instance.Info( "disconnecting..." );
                client.Disconnect();
            }
        }

        internal void SendMessage( string text ) {
            if ( IsConnected ) {
                client.SendMessage( text, dryRun );
            }
        }

        void Client_OnConnected( object sender, OnConnectedArgs e ) {
            YukarinetteLogger.Instance.Info( $"{e.BotUsername} connected in Twitch chat." );
        }

        void Client_OnDisconnected( object sender, OnDisconnectedArgs e ) {
            YukarinetteLogger.Instance.Info( $"{e.BotUsername} disconnected in Twitch chat." );
        }

        void Client_OnLog( object sender, OnLogArgs e ) {
            //YukarinetteLogger.Instance.Info( e.Data );
        }

        void Client_OnConnectionError( object sender, OnConnectionErrorArgs e ) {
            YukarinetteLogger.Instance.Error( e.Error );
        }

        ConnectionCredentials crentials = null;
        TwitchClient client = null;
        double latency = 10.0;
        bool dryRun = false;
    }
}
