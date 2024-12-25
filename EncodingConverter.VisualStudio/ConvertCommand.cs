global using DSPI = Microsoft.VisualStudio.Extensibility.VSSdkCompatibility.AsyncServiceProviderInjection<EnvDTE.DTE, EnvDTE.DTE>;
global using SSPI = Microsoft.VisualStudio.Extensibility.VSSdkCompatibility.AsyncServiceProviderInjection<Microsoft.VisualStudio.Shell.Interop.SVsStatusbar, Microsoft.VisualStudio.Shell.Interop.IVsStatusbar>;
using EncodingConverter.VisualStudio.Properties;
using Microsoft.VisualStudio.Extensibility;
using Microsoft.VisualStudio.Extensibility.Commands;
using Microsoft.VisualStudio.Extensibility.VSSdkCompatibility;
using Microsoft.VisualStudio.RpcContracts.Notifications;
using Microsoft.VisualStudio.Shell;
using System.IO;

namespace EncodingConverter.VisualStudio;

[VisualStudioContribution]
internal abstract class ConvertCommand(SSPI statusbarProvider) : Command {
    protected const string parentGuid = "{d309f791-903f-11d0-9efc-00a0c911004f}";

    /// <inheritdoc />
    public sealed override async Task ExecuteCommandAsync(IClientContext context, CancellationToken cancellationToken) {
        await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);

        ChooseEncodingControl control = new();

        if (await Extensibility.Shell().ShowDialogAsync((WpfControlWrapper)control, Resources.ChooseEncodingTitle, DialogOption.OKCancel, cancellationToken) == DialogResult.OK) {
            var items = await GetItemsAsync(cancellationToken);
            var array = items.ToArray();
            var statusbar = await statusbarProvider.GetServiceAsync();

            uint cookie = 0;

            // Initialize the progress bar
            statusbar.Progress(ref cookie, 1, string.Empty, 0, 0);

            for (var i = 0; i < array.Length; i++) {
                var file = array[i];

                // If file not exists
                if (!file.Exists) {
                    throw new FileNotFoundException("File Not Found!", file.FullName);
                }

                // Convert file
                await ConverterCore.ConvertEncodingAsync(file, control.ChosenEncoding);

                // Progress bar
                statusbar.Progress(ref cookie, 1, string.Format(Resources.StatusbarText, i + 1, array.Length), (uint)(i + 1), (uint)array.Length);

#pragma warning disable VSEXTPREVIEW_OUTPUTWINDOW // 형식은 평가 목적으로 제공되며, 이후 업데이트에서 변경되거나 제거될 수 있습니다. 계속하려면 이 진단을 표시하지 않습니다.
                // Print to Output window
                var channel = await context.Extensibility.Views().Output.CreateOutputChannelAsync("Encoding Converter", cancellationToken);

                await channel.WriteLineAsync(string.Format(Resources.OutputText, file.Name, i + 1, array.Length));
#pragma warning restore VSEXTPREVIEW_OUTPUTWINDOW // 형식은 평가 목적으로 제공되며, 이후 업데이트에서 변경되거나 제거될 수 있습니다. 계속하려면 이 진단을 표시하지 않습니다.
            }

            // Clear the progress bar
            statusbar.Progress(ref cookie, 0, string.Empty, 0, 0);
        }
    }

    protected abstract Task<IEnumerable<FileInfo>> GetItemsAsync(CancellationToken cancellationToken);
}
