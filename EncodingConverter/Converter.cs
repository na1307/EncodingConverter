using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EncodingConverter;

public static class Converter {
    public static async Task ConvertEncodingAsync(IEnumerable<FileInfo> files, Encoding newEncoding) {
        var array = files.ToArray();

        for (var i = 0; i < array.Length; i++) {
            var file = array[i];

            if (!file.Exists) {
                throw new FileNotFoundException("File Not Found!", file.FullName);
            }

            string fullText;

            using (StreamReader sr = new(file.FullName, await getEncodingAsync(file.FullName))) {
                fullText = await sr.ReadToEndAsync();
            }

            using (StreamWriter sw = new(file.FullName, false, newEncoding)) {
                await sw.WriteAsync(fullText);
            }

            await VS.StatusBar.ShowProgressAsync($"{i + 1}/{array.Length} file processed", i + 1, array.Length);
        }
    }

    /// <summary>
    /// Determines a text file's encoding by analyzing its byte order mark (BOM).
    /// Defaults to ASCII when detection of the text file's endianness fails.
    /// </summary>
    /// <param name="filename">The text file to analyze.</param>
    /// <returns>The detected encoding.</returns>
    private static async Task<Encoding> getEncodingAsync(string filename) {
        // Read the BOM
        var bom = new byte[4];

        using (FileStream file = new(filename, FileMode.Open, FileAccess.Read)) {
            await file.ReadAsync(bom, 0, 4);
        }

        // Analyze the BOM
        return bom[0] switch {
            0x2b when bom[1] == 0x2f && bom[2] == 0x76 => Encoding.UTF7,
            0xef when bom[1] == 0xbb && bom[2] == 0xbf => Encoding.UTF8,
            0xff when bom[1] == 0xfe && bom[2] == 0 && bom[3] == 0 => Encoding.UTF32, // UTF-32LE
            0xff when bom[1] == 0xfe => Encoding.Unicode, // UTF-16LE
            0xfe when bom[1] == 0xff => Encoding.BigEndianUnicode, // UTF-16BE
            0 when bom[1] == 0 && bom[2] == 0xfe && bom[3] == 0xff => new UTF32Encoding(true, true), // UTF-32BE
            _ => Encoding.Default, // We actually have no idea what the encoding is if we reach this point, so
        };                         // you may wish to return null instead of defaulting to ASCII
    }
}
