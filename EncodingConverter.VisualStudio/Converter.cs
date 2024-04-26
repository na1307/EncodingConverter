using Microsoft.VisualStudio;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace EncodingConverter.VisualStudio;

public static class Converter {
    public static async Task ConvertEncodingAsync(IEnumerable<FileInfo> files, Encoding newEncoding) {
        var array = files.ToArray();

        for (var i = 0; i < array.Length; i++) {
            var file = array[i];

            // If file not exists
            if (!file.Exists) {
                throw new FileNotFoundException("File Not Found!", file.FullName);
            }

            await ConverterCore.ConvertEncodingAsync(file, newEncoding);

            // Progress bar
            var pbTask = VS.StatusBar.ShowProgressAsync($"{i + 1}/{array.Length} file processed", i + 1, array.Length);

            // Print to Output window
            var pane = await OutputWindowPane.GetAsync(VSConstants.GUID_OutWindowGeneralPane);
            var owTask = pane.WriteLineAsync($"EncodingConverter: {file.Name} processed ({i + 1}/{array.Length})");

            await Task.WhenAll(pbTask, owTask);
        }
    }
}
