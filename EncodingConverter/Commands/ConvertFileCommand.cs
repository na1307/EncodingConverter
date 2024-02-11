using EnvDTE;
using System.Collections.Generic;
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

        var dte = (DTE)await Package.GetServiceAsync(typeof(DTE));
        IEnumerable<FileInfo> items = dte.SelectedItems.Cast<SelectedItem>().Select(item => {
            ThreadHelper.ThrowIfNotOnUIThread();
            return new FileInfo((string)item.ProjectItem.Properties.Item("FullPath").Value);
        });

        await Converter.ConvertEncodingAsync(items, dialog.ChosenEncoding);
    }
}
