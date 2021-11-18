using System;

using GtaModdingCli.Common;

namespace GtaModdingCli
{
    class Program
    {
        private static void Main(string[] args)
        {
            Cli cli = new();

            if (cli.CommandsCount == 0)
                throw new Exception("CLI does not export any commands");

            cli.Execute(args.Length == 0 ? new []{ "help" } : args);
        }
    }
}
