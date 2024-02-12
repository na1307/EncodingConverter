using System.Text;
using System.Windows;

namespace EncodingConverter;

/// <summary>
/// ChooseEncodingDialog.xaml에 대한 상호 작용 논리
/// </summary>
public partial class ChooseEncodingDialog {
    //private const int utf8CodePage = 65001;

    public ChooseEncodingDialog() {
        InitializeComponent();
        //encodingsCombo.ItemsSource = Encoding.GetEncodings().Select(ei => $"{ei.DisplayName} ({ei.CodePage})");
        encodingsCombo.ItemsSource = new string[] { "System Encoding", "UTF-8 with BOM", "UTF-8 without BOM" };
        encodingsCombo.SelectedIndex = encodingsCombo.Items.Count - 1;
    }

    public Encoding ChosenEncoding {
        get {
            //var e = Encoding.GetEncodings()[encodingsCombo.SelectedIndex].GetEncoding();

            //return e.CodePage is utf8CodePage ? new UTF8Encoding(bomCheckBox.IsChecked.GetValueOrDefault()) : e;

            return encodingsCombo.SelectedIndex switch {
                0 => Encoding.Default,
                1 => new UTF8Encoding(encoderShouldEmitUTF8Identifier: true),
                2 => new UTF8Encoding(encoderShouldEmitUTF8Identifier: false),
                _ => throw new InvalidOperationException(),
            };
        }
    }

    private void okButton_Click(object sender, RoutedEventArgs e) => DialogResult = true;
    private void cancelButton_Click(object sender, RoutedEventArgs e) => DialogResult = false;

    private void encodingsCombo_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e) {
        bomCheckBox.IsChecked = false;
        //bomCheckBox.Visibility = Encoding.GetEncodings()[encodingsCombo.SelectedIndex].CodePage is utf8CodePage ? Visibility.Visible : Visibility.Hidden;
        bomCheckBox.Visibility = Visibility.Hidden;
    }
}
