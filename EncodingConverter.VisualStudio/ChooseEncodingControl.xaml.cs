using System.Text;

namespace EncodingConverter.VisualStudio;

/// <summary>
/// ChooseEncodingControl.xaml에 대한 상호 작용 논리
/// </summary>
public sealed partial class ChooseEncodingControl {
    private static readonly string[] Encodings = [Properties.Resources.SystemEncoding, Properties.Resources.Utf8WithBom, Properties.Resources.Utf8WithoutBom];
    private static readonly UTF8Encoding Utf8WithBom = new(encoderShouldEmitUTF8Identifier: true);
    private static readonly UTF8Encoding Utf8WithoutBom = new(encoderShouldEmitUTF8Identifier: false);

    public ChooseEncodingControl() {
        InitializeComponent();
        encodingsCombo.ItemsSource = Encodings;
        encodingsCombo.SelectedIndex = encodingsCombo.Items.Count - 1;
    }

    public Encoding ChosenEncoding => encodingsCombo.SelectedIndex switch {
        0 => Encoding.Default, // System Encoding
        1 => Utf8WithBom,      // UTF-8 with BOM
        2 => Utf8WithoutBom,   // UTF-8 without BOM
        _ => throw new InvalidOperationException(),
    };
}
