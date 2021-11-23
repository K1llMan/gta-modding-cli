using System;
using System.IO;

using GtaModdingCli.Commands.Classes;
using GtaModdingCli.Common;

namespace GtaModdingCli.Commands
{
    [CliCommand("Radio builder command.",
        "Usage: radio [asset] [tracks] [pak-path] [mod-name]\n" +
        "asset: Uasset file name.\n" +
        "tracks: Path to tracks assets directory.\n" +
        "pak-path: Path inside pak.\n" +
        "mod-name: Mod file name.",
        "radio"
    )]
    public class Radio : AbstractCliCommand
    {
        private void ClearTemp()
        {
            if (Directory.Exists("temp"))
                Directory.Delete("temp", true);
        }

        public override void Execute(string[] args)
        {
            if (args.Length < 4)
            {
                Console.WriteLine("Arguments required: [asset] [tracks] [pak-path] [mod-name]");
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

            cli.ExecutePak(Path.Combine(AppContext.BaseDirectory, "temp"), args[3]);

            File.Move(Path.Combine(AppContext.BaseDirectory, "temp", $"{args[3]}.pak"), Path.Combine(AppContext.BaseDirectory, $"{args[3]}.pak"));
            ClearTemp();

            Console.WriteLine($"Created radio mod: {args[3]}");
        }

        public Radio(Cli cli) : base(cli)
        {

        }
    }
}
