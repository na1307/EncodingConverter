using EnvDTE;
using Microsoft.VisualStudio.Extensibility;
using Microsoft.VisualStudio.Extensibility.Commands;
using Microsoft.VisualStudio.Shell;
using System.IO;

namespace EncodingConverter.VisualStudio;

/// <summary>
/// ConvertFileCommand handler.
/// </summary>
[VisualStudioContribution]
internal sealed class ConvertFileCommand(SSPI statusbarProvider, DSPI dteProvider) : ConvertCommand(statusbarProvider) {
    [VisualStudioContribution]
    public static CommandGroupConfiguration ConvertFileGroup => new(GroupPlacement.VsctParent(Guid.Parse(ParentGuid), 0x0430, 0x0600)) {
        Children = [GroupChild.Command<ConvertFileCommand>()]
    };

    /// <inheritdoc />
    public override CommandConfiguration CommandConfiguration => new("%EncodingConverter.VisualStudio.ConvertFileCommand.DisplayName%") {
        // Use this object initializer to set optional parameters for the command. The required parameter,
        // displayName, is set above. DisplayName is localized and references an entry in .vsextension\string-resources.json.
        Icon = new(ImageMoniker.KnownValues.FileEncodingDialog, IconSettings.IconAndText),
    };

    protected override async Task<IEnumerable<FileInfo>> GetItemsAsync(CancellationToken cancellationToken) {
        await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);

        var dte = await dteProvider.GetServiceAsync();

        return dte.SelectedItems.Cast<SelectedItem>().Where(item => {
            ThreadHelper.ThrowIfNotOnUIThread();
            item.ProjectItem.Open();
            return item.ProjectItem.Document.Kind == Constants.vsDocumentKindText;
        }).Select(item => {
            ThreadHelper.ThrowIfNotOnUIThread();
            return new FileInfo((string)item.ProjectItem.Properties.Item("FullPath").Value);
        });
    }
}
