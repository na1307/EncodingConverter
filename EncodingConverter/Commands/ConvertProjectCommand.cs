using EnvDTE;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace EncodingConverter;

[Command(PackageIds.ConvertProjectCommand)]
internal sealed class ConvertProjectCommand : BaseCommand<ConvertProjectCommand> {
    protected override async Task ExecuteAsync(OleMenuCmdEventArgs e) {
        await Package.JoinableTaskFactory.SwitchToMainThreadAsync();

        ChooseEncodingDialog dialog = new();

        if (!dialog.ShowModal().GetValueOrDefault()) {
            return;
        }

        var dte = (DTE)await Package.GetServiceAsync(typeof(DTE));

        IEnumerable<FileInfo> items = dte.SelectedItems.Item(1).Project.ProjectItems.Cast<ProjectItem>().Where(item => {
            ThreadHelper.ThrowIfNotOnUIThread();
            item.Open();
            return item.Document.Kind == Constants.vsDocumentKindText;
        }).Select(item => {
            ThreadHelper.ThrowIfNotOnUIThread();
            return new FileInfo((string)item.Properties.Item("FullPath").Value);
        });

        await Converter.ConvertEncodingAsync(items, dialog.ChosenEncoding);
    }
}
