﻿using EnvDTE;
using Microsoft.VisualStudio.Extensibility;
using Microsoft.VisualStudio.Extensibility.Commands;
using Microsoft.VisualStudio.Extensibility.VSSdkCompatibility;
using Microsoft.VisualStudio.Shell;
using System.IO;

namespace EncodingConverter.VisualStudio;

/// <summary>
/// ConvertProjectCommand handler.
/// </summary>
[VisualStudioContribution]
internal sealed class ConvertProjectCommand(AsyncServiceProviderInjection<DTE, DTE> dteProvider) : Microsoft.VisualStudio.Extensibility.Commands.Command {
    [VisualStudioContribution]
    public static CommandGroupConfiguration ConvertProjectGroup => new(GroupPlacement.VsctParent(Guid.Parse("{d309f791-903f-11d0-9efc-00a0c911004f}"), 0x0402, 0x0600)) {
        Children = [GroupChild.Command<ConvertProjectCommand>()]
    };

    /// <inheritdoc />
    public override CommandConfiguration CommandConfiguration => new("%EncodingConverter.VisualStudio.ConvertProjectCommand.DisplayName%") {
        // Use this object initializer to set optional parameters for the command. The required parameter,
        // displayName, is set above. DisplayName is localized and references an entry in .vsextension\string-resources.json.
        Icon = new(ImageMoniker.KnownValues.FileEncodingDialog, IconSettings.IconAndText),
    };

    /// <inheritdoc />
    public override async Task ExecuteCommandAsync(IClientContext context, CancellationToken cancellationToken) {
        await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);

        var (result, encoding) = ChooseEncodingDialog.ShowDialogAndGetEncoding();

        if (!result) {
            return;
        }

        var dte = await dteProvider.GetServiceAsync();
        var items = dte.SelectedItems.Item(1).Project.ProjectItems.Cast<ProjectItem>().Where(item => {
            ThreadHelper.ThrowIfNotOnUIThread();

            if (!item.Kind.Equals("{6bb5f8ee-4483-11d3-8bcf-00c04f8ec28c}", StringComparison.OrdinalIgnoreCase)) {
                return false;
            }

            item.Open();
            return item.Document.Kind == Constants.vsDocumentKindText;
        }).Select(item => {
            ThreadHelper.ThrowIfNotOnUIThread();
            return new FileInfo((string)item.Properties.Item("FullPath").Value);
        });

        await Converter.ConvertEncodingAsync(items, encoding);
    }
}