using System.Text;
using System.Windows;

namespace EncodingConverter.VisualStudio;

/// <summary>
/// ChooseEncodingDialog.xaml에 대한 상호 작용 논리
/// </summary>
public partial class ChooseEncodingDialog {
    private static readonly string[] encodings = ["System Encoding", "UTF-8 with BOM", "UTF-8 without BOM"];
    private static readonly UTF8Encoding utf8withBom = new(encoderShouldEmitUTF8Identifier: true);
    private static readonly UTF8Encoding utf8withoutBom = new(encoderShouldEmitUTF8Identifier: false);

    private ChooseEncodingDialog() {
        InitializeComponent();
        encodingsCombo.ItemsSource = encodings;
        encodingsCombo.SelectedIndex = encodingsCombo.Items.Count - 1;
    }

    public static (bool dialogResult, Encoding chosen) ShowDialogAndGetEncoding() {
        ChooseEncodingDialog dialog = new();
        var result = dialog.ShowModal().GetValueOrDefault();
        var encoding = result ? dialog.encodingsCombo.SelectedIndex switch {
            0 => Encoding.Default, // System Encoding
            1 => utf8withBom,      // UTF-8 with BOM
            2 => utf8withoutBom,   // UTF-8 without BOM
            _ => throw new InvalidOperationException(),
        } : null;

        return (result, encoding);
    }

    private void okButton_Click(object sender, RoutedEventArgs e) => DialogResult = true;
    private void cancelButton_Click(object sender, RoutedEventArgs e) => DialogResult = false;
}
