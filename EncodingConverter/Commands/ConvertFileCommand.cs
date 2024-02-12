using EnvDTE;
using System.IO;
using System.Linq;

namespace EncodingConverter;

[Command(PackageIds.ConvertFileCommand)]
internal sealed class ConvertFileCommand : BaseCommand<ConvertFileCommand> {
    protected override async Task ExecuteAsync(OleMenuCmdEventArgs e) {
        await Package.JoinableTaskFactory.SwitchToMainThreadAsync();

        ChooseEncodingDialog dialog = new();

        if (!dialog.ShowModal().GetValueOrDefault()) {
            return;
        }

        var dte = (await Package.GetServiceAsync(typeof(DTE)) as DTE) ?? throw new InvalidOperationException();
        var items = dte.SelectedItems.Cast<SelectedItem>().Where(item => {
            ThreadHelper.ThrowIfNotOnUIThread();
            item.ProjectItem.Open();
            return item.ProjectItem.Document.Kind == Constants.vsDocumentKindText;
        }).Select(item => {
            ThreadHelper.ThrowIfNotOnUIThread();
            return new FileInfo((string)item.ProjectItem.Properties.Item("FullPath").Value);
        });

        await Converter.ConvertEncodingAsync(items, dialog.ChosenEncoding);
    }
}
