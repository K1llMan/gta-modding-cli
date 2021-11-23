using System;

using GtaModdingCli.Common;

namespace GtaModdingCli.Commands
{
    [CliCommand("Pak command.",
        "Usage: pak [directory] [pak-name]\n" + 
        "directory: Directory to pak.\n" +
        "pak-name: Name of .pak without extension.",
        "pak"
    )]
    public class Packer : AbstractCliCommand
    {
        public override void Execute(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Arguments required: [directory] [pak-name]");
                return;
            }

            cli.ExecutePak(args[0], args[1]);
        }

        public Packer(Cli cli) : base(cli)
        {

        }
    }
}
