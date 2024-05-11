﻿using Microsoft.VisualStudio.Extensibility;
using Microsoft.VisualStudio.Extensibility.Commands;

namespace EncodingConverter.VisualStudio;

/// <summary>
/// ConvertFileCommand handler.
/// </summary>
[VisualStudioContribution]
internal sealed class ConvertFileCommand : Command {
    [VisualStudioContribution]
    public static CommandGroupConfiguration ConvertFileGroup => new(GroupPlacement.VsctParent(Guid.Parse("{d309f791-903f-11d0-9efc-00a0c911004f}"), 0x0430, 0x0600)) {
        Children = [GroupChild.Command<ConvertFileCommand>()]
    };

    /// <inheritdoc />
    public override CommandConfiguration CommandConfiguration => new("%EncodingConverter.VisualStudio.ConvertFileCommand.DisplayName%") {
        // Use this object initializer to set optional parameters for the command. The required parameter,
        // displayName, is set above. DisplayName is localized and references an entry in .vsextension\string-resources.json.
        Icon = new(ImageMoniker.KnownValues.FileEncodingDialog, IconSettings.IconAndText),
    };

    /// <inheritdoc />
    public override async Task ExecuteCommandAsync(IClientContext context, CancellationToken cancellationToken) {

    }
}
