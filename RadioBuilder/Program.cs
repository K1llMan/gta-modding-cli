using System;
using System.Diagnostics;
using System.IO;

namespace RadioBuilder
{
    class Program
    {
        private static void ClearTemp()
        {
            if (Directory.Exists("temp"))
                Directory.Delete("temp", true);
        }

        static void Main(string[] args)
        {
            if (args.Length < 4)
            {
                Console.WriteLine("Arguments required: <radio station uasset file path> <path to tracks> <path inside .pak> <output mod filename>");
                return;
            }

            ClearTemp();

            RadioBuilder builder = new();
            builder.Load(args[0]);

            builder.AddTracks(args[1], args[2]);

            string tempDir = $"temp\\{args[2].Replace("/", "\\").TrimStart('\\').Replace("Game", "Gameface\\Content")}";
            Directory.CreateDirectory(tempDir);
            foreach (string file in Directory.GetFiles(args[1]))
            {
                File.Copy(file, $"{tempDir}\\{Path.GetFileName(file)}");
            }

            builder.Save($"{tempDir}\\{Path.GetFileName(args[0])}");

            Process process = new() {
                EnableRaisingEvents = true,
                StartInfo = new() {
                    FileName = Path.Combine(AppContext.BaseDirectory, "unrealPak", "UnrealPak-With-Compression.bat"),
                    Arguments =  Path.Combine(AppContext.BaseDirectory, "temp"),
                    WorkingDirectory = Path.Combine(AppContext.BaseDirectory, "temp"),
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            process.OutputDataReceived += (s, e) => {
                Console.WriteLine(e.Data);
            };

            process.Start();
            process.BeginOutputReadLine();
            process.WaitForExit();

            File.Move(Path.Combine(AppContext.BaseDirectory, "temp.pak"), args[3]);
            ClearTemp();

            Console.WriteLine($"Created radio mod: {args[3]}");
            Console.ReadKey();
        }
    }
}
