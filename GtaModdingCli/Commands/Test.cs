using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;

using GtaModdingCli.Common;

using Sharprompt;

using UAssetAPI;
using UAssetAPI.UnrealTypes.Enums;

namespace GtaModdingCli.Commands
{
    [CliCommand("Test asset.",
        "Usage: test [asset]\n" +
        "asset: Path to asset.",
        "test"
    )]
    public class Test : AbstractCliCommand
    {
        public override void Execute(string[] args)
        {
            UAsset asset = new(args[0], UE4Version.VER_UE4_26);
        }

        public override string[] GetInteractiveData()
        {
            return new[] { 
                Prompt.Input<string>("Path to asset", validators: new List<Func<object, ValidationResult>> { FileExists })
            };
        }

        public Test(Cli cli) : base(cli)
        {

        }
    }
}
