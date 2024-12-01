using System.Text;

namespace EncodingConverter.VisualStudio;

/// <summary>
/// ChooseEncodingControl.xaml에 대한 상호 작용 논리
/// </summary>
public sealed partial class ChooseEncodingControl {
    private static readonly string[] encodings = [Properties.Resources.SystemEncoding, Properties.Resources.Utf8WithBom, Properties.Resources.Utf8WithoutBom];
    private static readonly UTF8Encoding utf8withBom = new(encoderShouldEmitUTF8Identifier: true);
    private static readonly UTF8Encoding utf8withoutBom = new(encoderShouldEmitUTF8Identifier: false);

    public ChooseEncodingControl() {
        InitializeComponent();
        encodingsCombo.ItemsSource = encodings;
        encodingsCombo.SelectedIndex = encodingsCombo.Items.Count - 1;
    }

    public Encoding ChosenEncoding => encodingsCombo.SelectedIndex switch {
        0 => Encoding.Default, // System Encoding
        1 => utf8withBom,      // UTF-8 with BOM
        2 => utf8withoutBom,   // UTF-8 without BOM
        _ => throw new InvalidOperationException(),
    };
}
