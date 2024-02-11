using Microsoft.VisualStudio.PlatformUI;
using System.Linq;
using System.Text;
using System.Windows;

namespace EncodingConverter;

/// <summary>
/// ChooseEncodingDialog.xaml에 대한 상호 작용 논리
/// </summary>
public partial class ChooseEncodingDialog : DialogWindow {
    private const int utf8CodePage = 65001;

    public ChooseEncodingDialog() {
        InitializeComponent();
        encodingsCombo.ItemsSource = Encoding.GetEncodings().Select(ei => $"{ei.DisplayName} ({ei.CodePage})");
        encodingsCombo.SelectedIndex = 0;
    }

    public Encoding ChosenEncoding {
        get {
            Encoding e = Encoding.GetEncodings()[encodingsCombo.SelectedIndex].GetEncoding();

            return e.CodePage is utf8CodePage ? new UTF8Encoding(bomCheckBox.IsChecked.GetValueOrDefault()) : e;
        }
    }

    private void okButton_Click(object sender, RoutedEventArgs e) => DialogResult = true;
    private void cancelButton_Click(object sender, RoutedEventArgs e) => DialogResult = false;

    private void encodingsCombo_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e) {
        bomCheckBox.IsChecked = false;
        bomCheckBox.Visibility = Encoding.GetEncodings()[encodingsCombo.SelectedIndex].GetEncoding().CodePage is utf8CodePage ? Visibility.Visible : Visibility.Hidden;
    }
}
