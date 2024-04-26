using EnvDTE;
using System.IO;
using System.Linq;

namespace EncodingConverter;

[Command(PackageIds.ConvertProjectCommand)]
internal sealed class ConvertProjectCommand : BaseCommand<ConvertProjectCommand> {
    protected override async Task ExecuteAsync(OleMenuCmdEventArgs e) {
        await Package.JoinableTaskFactory.SwitchToMainThreadAsync();

        var (result, encoding) = ChooseEncodingDialog.ShowDialogAndGetEncoding();

        if (!result) {
            return;
        }

        var dte = (await Package.GetServiceAsync(typeof(DTE)) as DTE) ?? throw new InvalidOperationException();
        var items = dte.SelectedItems.Item(1).Project.ProjectItems.Cast<ProjectItem>().Where(item => {
            ThreadHelper.ThrowIfNotOnUIThread();
            item.Open();
            return item.Document.Kind == Constants.vsDocumentKindText;
        }).Select(item => {
            ThreadHelper.ThrowIfNotOnUIThread();
            return new FileInfo((string)item.Properties.Item("FullPath").Value);
        });

        await Converter.ConvertEncodingAsync(items, encoding);
    }
}
