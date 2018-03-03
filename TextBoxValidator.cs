using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace TwitchChatPlugin {
    /// <summary>
    /// local:TextBoxValidator.IsNumeric = "True"
    /// で数値のみ、符号や小数点も許可しない
    /// 継承してもしなくても通じるが、その意味は？staticにしたいんだが
    /// 複数設定をもたせるにはこれにobjectを割り当てるのか？ static? 毎回 TextBoxValidator.Is*** と書くのはアホらしいぞ
    /// これを書いてる時点でTrue以外ありえないのに、そう書かなくてはならないのが間抜け。他の書き方無いのか？
    /// </summary>
    internal class TextBoxValidator : DependencyObject {
        #region IsNumeric

        [AttachedPropertyBrowsableForType( typeof( TextBox ) )]
        internal static bool GetIsNumeric( DependencyObject obj ) {
            return (bool)obj.GetValue( isNumericProperty );
        }

        [AttachedPropertyBrowsableForType( typeof( TextBox ) )]
        internal static void SetIsNumeric( DependencyObject obj, bool value ) {
            obj.SetValue( isNumericProperty, value );
        }

        internal static readonly DependencyProperty isNumericProperty =
            DependencyProperty.RegisterAttached(
                "IsNumeric",
                typeof( bool ),
                typeof( TextBoxValidator ),
                new UIPropertyMetadata( false, IsNumericChanged ) );


        private static void IsNumericChanged( DependencyObject sender, DependencyPropertyChangedEventArgs e ) {
            TextBox textBox = sender as TextBox;
            if ( textBox == null ) {
                return;
            }

            // イベントを登録・削除 
            bool newValue = (bool)e.NewValue;
            if ( newValue ) {
                textBox.PreviewTextInput += IsNumericPreviewTextInput;
                textBox.PreviewKeyDown += IsNumericPreviewKeyDown;
                DataObject.AddPastingHandler( textBox, IsNumericPastingEventHandler );
            } else {
                textBox.PreviewTextInput -= IsNumericPreviewTextInput;
                textBox.PreviewKeyDown -= IsNumericPreviewKeyDown;
                DataObject.RemovePastingHandler( textBox, IsNumericPastingEventHandler ); // for paste
            }
        }

        private static void IsNumericPreviewKeyDown( object sender, KeyEventArgs e ) {
            e.Handled = e.Key == Key.Space; // spaceはTextInputを素通りする…
        }

        private static void IsNumericPreviewTextInput( object sender, TextCompositionEventArgs e ) {
            e.Handled = !IsNmericAllowed( e.Text );
        }

        private static void IsNumericPastingEventHandler( object sender, DataObjectPastingEventArgs e ) {
            TextBox textBox = ( sender as TextBox );
            if ( textBox == null ) {
                e.CancelCommand();
                e.Handled = true;
            } else {
                string clipboard = e.DataObject.GetData( typeof( string ) ) as string;
                if ( !IsNmericAllowed( clipboard ) ) {
                    e.CancelCommand();
                    e.Handled = true;
                }
            }
        }

        private static bool IsNmericAllowed( string text ) {
            foreach ( var c in text ) {
                if ( char.IsNumber( c ) == false ) {
                    return false;
                }
            }
            return true;
        }

        #endregion

        // 他に小数点(point)、range min/maxがあるとよい
        // rangeがあれば符号の有無も考慮できる

    }
}
