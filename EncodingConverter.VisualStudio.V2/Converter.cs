using Microsoft.VisualStudio.Extensibility;
using System.IO;
using System.Text;

namespace EncodingConverter.VisualStudio;

internal static class Converter {
    public static async Task ConvertEncodingAsync(IEnumerable<FileInfo> files, Encoding newEncoding, IClientContext context, CancellationToken token) {
        var array = files.ToArray();

        for (var i = 0; i < array.Length; i++) {
            var file = array[i];

            // If file not exists
            if (!file.Exists) {
                throw new FileNotFoundException("File Not Found!", file.FullName);
            }

            await ConverterCore.ConvertEncodingAsync(file, newEncoding);

            // Progress bar
            //var pbTask = VS.StatusBar.ShowProgressAsync($"{i + 1}/{array.Length} file processed", i + 1, array.Length);

#pragma warning disable VSEXTPREVIEW_OUTPUTWINDOW // 형식은 평가 목적으로 제공되며, 이후 업데이트에서 변경되거나 제거될 수 있습니다. 계속하려면 이 진단을 표시하지 않습니다.
            // Print to Output window
            var channel = await context.Extensibility.Views().Output.GetChannelAsync("Encoding Converter", nameof(Properties.Resources.OWString), token);

            await channel.Writer.WriteLineAsync($"Encoding Converter: {file.Name} processed ({i + 1}/{array.Length})");
#pragma warning restore VSEXTPREVIEW_OUTPUTWINDOW // 형식은 평가 목적으로 제공되며, 이후 업데이트에서 변경되거나 제거될 수 있습니다. 계속하려면 이 진단을 표시하지 않습니다.

            //await Task.WhenAll(pbTask, owTask);
        }
    }
}
