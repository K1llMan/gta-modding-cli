using System;
using System.Linq;
using System.Reflection;

using GtaModdingCli.Common;

namespace GtaModdingCli.Commands
{
    [CliCommand("Help topic command. Return command list with short description or full command description.",
        "Usage: help|h [command]\n" + 
        "command: Empty string or command name",
        "help", "-h"
    )]
    public class Helper : AbstractCliCommand
    {
        private string GetFormattedDesc(CliCommandAttribute attr)
        {
            string list = string.Join("|", attr.Commands);

            return $"{list}: {attr.Description}\n  {string.Join("\n    ", attr.Usage.Split("\n"))}\n";
        }

        public override void Execute(string[] args)
        {
            // Short description
            if (args.Length == 0)
            {
                string[] descs = cli.Commands.Select(t => GetFormattedDesc(t.GetCustomAttribute<CliCommandAttribute>()))
                    .ToArray();

                Console.WriteLine("Commands list:\n");
                foreach (string desc in descs)
                {
                    Console.WriteLine(desc);
                }

                return;
            }

            Type type = cli.Commands.FirstOrDefault(t => t.GetCustomAttribute<CliCommandAttribute>().Commands.Contains(args[0]));
            if (type == null)
            {
                Console.WriteLine($"Unknown command name \"{args[0]}\"");
                return;
            }

            Console.WriteLine(GetFormattedDesc(type.GetCustomAttribute<CliCommandAttribute>()));
        }

        public Helper(Cli cli) : base(cli)
        {

        }
    }
}
