using System.Text;

namespace EncodingConverter.Tests;

internal sealed class Utf8WithBomEncoding : UTF8Encoding {
    public Utf8WithBomEncoding() : base(true) { }
}
