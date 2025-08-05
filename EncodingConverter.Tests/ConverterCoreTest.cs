using System.Text;

namespace EncodingConverter.Tests;

public sealed class ConverterCoreTest {
    private const string Text1 = "Tempor dolor lorem rebum facilisis ullamcorper amet dolores est qui eos ipsum no eirmod dolores autem aliquip sed nonummy elitr";
    private const string Text2 = "Light pleasure strength tis or she favour mood not what none pleasure it his were start flatterers strange ever to";
    private const string Text3 = "In the beginning God created the heaven and the earth. And the earth was without form, and void; and darkness was upon the face of the deep. And the Spirit of God moved upon the face of the waters";

    [Theory]
    [InlineData(Text1, typeof(UTF8Encoding), typeof(Utf8WithBomEncoding))]
    [InlineData(Text2, typeof(Utf8WithBomEncoding), typeof(UTF8Encoding))]
    [InlineData(Text3, typeof(Encoding), typeof(UTF8Encoding))]
    [InlineData(Text1, typeof(UTF8Encoding), typeof(Encoding))]
    [InlineData(Text2, typeof(Utf8WithBomEncoding), typeof(Encoding))]
    [InlineData(Text3, typeof(Encoding), typeof(Utf8WithBomEncoding))]
    public async Task TestAsync(string text, Type baseEncoding, Type convertedEncoding) {
        if (!baseEncoding.IsAssignableTo(typeof(Encoding))) {
            throw new ArgumentException($"{nameof(baseEncoding)} is not an Encoding type.");
        }

        if (!convertedEncoding.IsAssignableTo(typeof(Encoding))) {
            throw new ArgumentException($"{nameof(convertedEncoding)} is not an Encoding type.");
        }

        var realBaseEncoding = baseEncoding == typeof(Encoding) ? CodePagesEncodingProvider.Instance.GetEncoding(0)! : (Encoding)Activator.CreateInstance(baseEncoding)!;
        var realConvertedEncoding = convertedEncoding == typeof(Encoding) ? CodePagesEncodingProvider.Instance.GetEncoding(0)! : (Encoding)Activator.CreateInstance(convertedEncoding)!;
        var file = await PrepareAsync(text, realBaseEncoding);
        var beforeContent = await File.ReadAllTextAsync(file, realBaseEncoding, TestContext.Current.CancellationToken);

        await ConverterCore.ConvertEncodingAsync(new(file), realConvertedEncoding);

        var afterContent = await File.ReadAllTextAsync(file, realConvertedEncoding, TestContext.Current.CancellationToken);

        End(file);

        Assert.Equal(beforeContent, afterContent);
    }

    private static async Task<string> PrepareAsync(string text, Encoding encoding) {
        var filename = Path.Combine(AppContext.BaseDirectory, Path.GetRandomFileName());

        await File.WriteAllTextAsync(filename, text, encoding);

        return filename;
    }

    private static void End(string filename) => File.Delete(filename);
}
