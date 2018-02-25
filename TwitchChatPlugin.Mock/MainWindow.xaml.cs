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

namespace TwitchChatPlugin.Mock {
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
        }

        private void Button_Click( object sender, RoutedEventArgs e ) {
            //Console.WriteLine( Settings.SavedLocation );
            // for gui testing
            Settings settings = Settings.Load( Settings.Filename );
            var s = new SettingsWindow();
            if ( s.Show( Application.Current.MainWindow, settings ) ) {
                settings = s.Accept();
                Settings.Save( Settings.Filename,  settings );
            }
        }
    }
}
