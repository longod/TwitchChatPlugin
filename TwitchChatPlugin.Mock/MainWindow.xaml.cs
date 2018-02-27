using System.Windows;

namespace TwitchChatPlugin.Mock {
    /// <summary>
    /// Window for GUI Testing
    /// </summary>
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
        }

        private void Button_Click( object sender, RoutedEventArgs e ) {
            Settings settings = Settings.Load( Settings.Filename );
            var s = new SettingsWindow();
            if ( s.Show( Application.Current.MainWindow, settings ) ) {
                settings = s.Accept();
                Settings.Save( Settings.Filename,  settings );
            }
        }
    }
}
