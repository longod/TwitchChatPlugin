using System.Windows;

namespace TwitchChatPlugin.Mock {
    /// <summary>
    /// Window for GUI Testing
    /// </summary>
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
        }

        private void Settings_Click( object sender, RoutedEventArgs e ) {
            settings = Settings.Load( Settings.Filename );
            var s = new SettingsWindow();
            if ( s.Show( Application.Current.MainWindow, settings ) ) {
                settings = s.Accept();
                Settings.Save( Settings.Filename,  settings );
            }
        }

        private void Connect_Click( object sender, RoutedEventArgs e ) {
            if ( settings == null ) {
                settings = Settings.Load( Settings.Filename );
            }
            client = new ChatClient( settings.username, Cryptography.Decrypt( settings.oauth ), settings.Latency, settings.ProxyIP, settings.Port, true );
            client?.Connect();
        }

        private void Disconnect_Click( object sender, RoutedEventArgs e ) {
            client?.Disconnect();
            client = null; // dispose?
        }

        ChatClient client = null;
        Settings settings = null;

        private void Send_Click( object sender, RoutedEventArgs e ) {
            string text = textboxSend.Text;
            if ( text != null ) {
                client?.EnqueueMessage( text );
            }
        }
    }
}
