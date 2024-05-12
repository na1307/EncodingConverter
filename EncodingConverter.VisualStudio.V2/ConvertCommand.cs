using Microsoft.VisualStudio.Extensibility;
using Microsoft.VisualStudio.Extensibility.Commands;
using Microsoft.VisualStudio.Extensibility.VSSdkCompatibility;
using Microsoft.VisualStudio.RpcContracts.Notifications;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System.IO;

namespace EncodingConverter.VisualStudio;

[VisualStudioContribution]
internal abstract class ConvertCommand(AsyncServiceProviderInjection<SVsStatusbar, IVsStatusbar> statusbarProvider) : Command {
    /// <inheritdoc />
    public sealed override async Task ExecuteCommandAsync(IClientContext context, CancellationToken cancellationToken) {
        await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);

        ChooseEncodingControl control = new();

        if (await Extensibility.Shell().ShowDialogAsync((WpfControlWrapper)control, "Choose Encoding", DialogOption.OKCancel, cancellationToken) == DialogResult.OK) {
            var items = await GetItemsAsync(cancellationToken);
            var statusbar = await statusbarProvider.GetServiceAsync();

            await Converter.ConvertEncodingAsync(items, control.ChosenEncoding, context, statusbar, cancellationToken);
        }
    }

    protected abstract Task<IEnumerable<FileInfo>> GetItemsAsync(CancellationToken cancellationToken);
}
