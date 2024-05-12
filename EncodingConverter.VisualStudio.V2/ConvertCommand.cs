using Microsoft.VisualStudio.Extensibility;
using Microsoft.VisualStudio.Extensibility.Commands;
using Microsoft.VisualStudio.Extensibility.VSSdkCompatibility;
using Microsoft.VisualStudio.RpcContracts.Notifications;
using Microsoft.VisualStudio.Shell;
using System.Text;

namespace EncodingConverter.VisualStudio;

[VisualStudioContribution]
internal abstract class ConvertCommand : Command {
    /// <inheritdoc />
    public sealed override async Task ExecuteCommandAsync(IClientContext context, CancellationToken cancellationToken) {
        await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);

        ChooseEncodingControl control = new();

        if (await Extensibility.Shell().ShowDialogAsync((WpfControlWrapper)control, "Choose Encoding", DialogOption.OKCancel, cancellationToken) == DialogResult.OK) {
            await ExecuteCommandCoreAsync(control.ChosenEncoding, context, cancellationToken);
        }
    }

    protected abstract Task ExecuteCommandCoreAsync(Encoding encoding, IClientContext context, CancellationToken cancellationToken);
}
