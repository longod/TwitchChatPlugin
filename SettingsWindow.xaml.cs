using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;

namespace TwitchChatPlugin {
    /// <summary>
    /// Settings.xaml の相互作用ロジック
    /// </summary>
    public partial class SettingsWindow : Window {
        public SettingsWindow() {
            InitializeComponent();
        }

        internal bool Show( Window owner, Settings settings ) {
            Owner = owner;
            textboxUsername.Text = settings.username;
            passwordboxOAuth.Password = Cryptography.Decrypt( settings.oauth );
            sliderLatency.Value = settings.Latency;
            checkboxEnableProxy.IsChecked = settings.enableProxy;
            textboxAddress.Text = settings.proxyIP;
            textboxAddress.Text = settings.proxyPort;
            return ShowDialog().Value;
        }
        internal Settings Accept() {
            var s = new Settings {
                username = textboxUsername.Text,
                oauth = Cryptography.Encrypt( passwordboxOAuth.Password ),
                latency = sliderLatency.Value.ToString("0.0"),
                enableProxy = checkboxEnableProxy.IsChecked ?? false,
                proxyIP = textboxAddress.Text,
                proxyPort = textboxPort.Text,
            };
            return s;
        }

        private void Hyperlink_RequestNavigate( object sender, RequestNavigateEventArgs e ) {
            Process.Start( new ProcessStartInfo( e.Uri.AbsoluteUri ) );
            e.Handled = true;
        }

        private void ButtonOK_Click( object sender, RoutedEventArgs e ) {
            DialogResult = true;
        }
    }
}
