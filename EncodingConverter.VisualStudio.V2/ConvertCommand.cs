﻿using Microsoft.VisualStudio.Extensibility;
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
            var array = items.ToArray();
            var statusbar = await statusbarProvider.GetServiceAsync();

            uint cookie = 0;

            // Initialize the progress bar
            statusbar.Progress(ref cookie, 1, "", 0, 0);

            for (var i = 0; i < array.Length; i++) {
                var file = array[i];

                // If file not exists
                if (!file.Exists) {
                    throw new FileNotFoundException("File Not Found!", file.FullName);
                }

                // Convert file
                await ConverterCore.ConvertEncodingAsync(file, control.ChosenEncoding);

                // Progress bar
                statusbar.Progress(ref cookie, 1, $"{i + 1}/{array.Length} file processed", (uint)(i + 1), (uint)array.Length);

#pragma warning disable VSEXTPREVIEW_OUTPUTWINDOW // 형식은 평가 목적으로 제공되며, 이후 업데이트에서 변경되거나 제거될 수 있습니다. 계속하려면 이 진단을 표시하지 않습니다.
                // Print to Output window
                var channel = await context.Extensibility.Views().Output.GetChannelAsync("Encoding Converter", nameof(Properties.Resources.OWString), cancellationToken);

                await channel.Writer.WriteLineAsync($"Encoding Converter: {file.Name} processed ({i + 1}/{array.Length})");
#pragma warning restore VSEXTPREVIEW_OUTPUTWINDOW // 형식은 평가 목적으로 제공되며, 이후 업데이트에서 변경되거나 제거될 수 있습니다. 계속하려면 이 진단을 표시하지 않습니다.
            }

            // Clear the progress bar
            statusbar.Progress(ref cookie, 0, "", 0, 0);
        }
    }

    protected abstract Task<IEnumerable<FileInfo>> GetItemsAsync(CancellationToken cancellationToken);
}
