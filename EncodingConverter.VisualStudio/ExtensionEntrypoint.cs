using Microsoft.VisualStudio.Extensibility;
using System.Resources;

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

    protected override ResourceManager? ResourceManager => Properties.Resources.ResourceManager;
}
