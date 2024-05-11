using Microsoft.VisualStudio.Extensibility;

namespace EncodingConverter.VisualStudio;

/// <summary>
/// Extension entrypoint for the VisualStudio.Extensibility extension.
/// </summary>
[VisualStudioContribution]
internal sealed class ExtensionEntrypoint : Extension {
    /// <inheritdoc />
    public override ExtensionConfiguration ExtensionConfiguration => new() {
        RequiresInProcessHosting = true,
    };
}
