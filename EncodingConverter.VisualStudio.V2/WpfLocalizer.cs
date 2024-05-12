using EncodingConverter.VisualStudio.Properties;
using System.Resources;

namespace EncodingConverter.VisualStudio;

public sealed class WpfLocalizer {
    private static readonly ResourceManager _resourceManager = Resources.ResourceManager;

    public string? this[string id] {
        get {
            if (string.IsNullOrWhiteSpace(id)) {
                return null;
            }

            //1. 리소스에서 값 조회
            var str = _resourceManager.GetString(id, Resources.Culture);

            //2. 없으면 키 반환
            if (string.IsNullOrWhiteSpace(str)) {
                str = id;
            }

            return str;
        }
    }
}
