using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;

using GtaModdingCli.Common;

using Sharprompt;

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

        public override string[] GetInteractiveData()
        {
            return new[] { 
                Prompt.Input<string>("Directory to pak", validators: new List<Func<object, ValidationResult>> { DirectoryExists }),
                Prompt.Input<string>("Pak name without extension", validators: new List<Func<object, ValidationResult>> {
                    Validators.Required("String must be non empty.")
                })
            };
        }

        public Packer(Cli cli) : base(cli)
        {

        }
    }
}
