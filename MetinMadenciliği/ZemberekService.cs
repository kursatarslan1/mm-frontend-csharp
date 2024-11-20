using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

public class ZemberekService
{
    private readonly string zemberekJarPath = Path.GetFullPath(
        Path.Combine(Directory.GetCurrentDirectory(), @"..\..\libs\zemberek-full.jar")
    );

    public string AnalyzeText(string text)
    {
        try
        {
            // Java komutunu çalıştır
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "java",
                    Arguments = $"-Xms512m -Xmx2g -Dfile.encoding=UTF-8 -cp \"{zemberekJarPath};.\" TestZemberek \"{text}\"",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    WorkingDirectory = Path.GetDirectoryName(Application.ExecutablePath)
                }
            };

            string output = string.Empty;
            string error = string.Empty;

            process.OutputDataReceived += (sender, args) => {
                if (args.Data != null)
                    output += args.Data + Environment.NewLine;
            };

            process.ErrorDataReceived += (sender, args) => {
                if (args.Data != null)
                    error += args.Data + Environment.NewLine;
            };

            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            process.WaitForExit();

            // Hata mesajını kontrol et
            if (!string.IsNullOrEmpty(error))
            {
                throw new Exception("Java Hata: " + error);
            }

            return output;
        }
        catch (Exception ex)
        {
            throw new Exception("Zemberek çalıştırılırken hata oluştu: " + ex.Message);
        }
    }


}
