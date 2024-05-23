using EncodingConverter.VisualStudio.Properties;

namespace EncodingConverter.VisualStudio;

public sealed class WpfLocalizer {

    public string? this[string id] {
        get {
            if (string.IsNullOrWhiteSpace(id)) {
                return null;
            }

            // 1. 리소스에서 값 조회
            var str = Resources.ResourceManager.GetString(id, Resources.Culture);

            // 2. 없으면 키 반환
            if (string.IsNullOrWhiteSpace(str)) {
                str = id;
            }

            return str;
        }
    }
}
