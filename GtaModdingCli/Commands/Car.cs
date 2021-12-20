using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using GtaModdingCli.Commands.Classes;
using GtaModdingCli.Common;

using Sharprompt;

using UAssetAPI;
using UAssetAPI.UnrealTypes.Enums;

namespace GtaModdingCli.Commands
{
    [CliCommand("Car asset commands.",
        "Usage: car [asset] [wheel mesh]\n" +
        "asset: Path to asset.\n" +
        "wheel mesh: Path to wheel mesh asset.",
        "car"
    )]
    public class Car : AbstractCliCommand
    {
        public override void Execute(string[] args)
        {
            CarEditor editor = new();
            editor.AddWheel(args[0], args[1]);
        }

        public override string[] GetInteractiveData()
        {
            return new[] { 
                Prompt.Input<string>("Path to asset", validators: new List<Func<object, ValidationResult>> { FileExists }),
                Prompt.Input<string>("Path to wheel mesh asset", validators: new List<Func<object, ValidationResult>> { Validators.Required() }),
            };
        }

        public Car(Cli cli) : base(cli)
        {

        }
    }
}
