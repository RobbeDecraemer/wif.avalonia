using System.IO;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Collections.Generic;
using Avalonia.Media.Imaging;

namespace WifViewer
{
    public class MovieExporter
    {
        public static async Task Export(List<WriteableBitmap> frames, int fps, string outputFile)
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = Configuration.FFMPEG_PATH,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    Arguments = $"-y -framerate {fps} -vcodec png -i - -c:v libx264 -r {fps} -pix_fmt yuv420p {outputFile}"
                }
            };

            process.Start();

            using (var binaryWriter = new BinaryWriter(process.StandardInput.BaseStream))
            {
                foreach (var frame in frames)
                {
            
                    var memoryStream = new MemoryStream();
                    frame.Save(memoryStream);
                    memoryStream.Seek(0, SeekOrigin.Begin);
                    var buffer = new byte[memoryStream.Length];
                    memoryStream.Read(buffer, 0, buffer.Length);
                    binaryWriter.Write(buffer, 0, buffer.Length);
                }
            }

            var output = await process.StandardOutput.ReadToEndAsync();
            await process.WaitForExitAsync();

            Debug.WriteLine(output);
        }
    }
}
