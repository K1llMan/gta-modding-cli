using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using GtaModdingCli.Common;
using UAssetAPI;
using UAssetAPI.PropertyTypes;
using UAssetAPI.StructTypes;

namespace GtaModdingCli.Commands
{
    [CliCommand("Help topic command. Return command list with short description or full command description.",
        "Usage: help|h [command]\n" + 
        "command: Empty string or command name",
        "material"
    )]
    public class Material : AbstractCliCommand
    {
        public override void Execute(string[] args)
        {
            UAsset asset = new UAsset(args[0], UE4Version.VER_UE4_26);



            NormalExport ne = (NormalExport) asset.Exports.First();
            if (ne.Data.Last() is ArrayPropertyData ad)
            {
                PropertyData propertyDatas = ((StructPropertyData) ad.Value[0]).Value.First(p => p.Name.Value.Value == "TextureName");
                FString newPath = new FString("/Game/GTA3/Textures/Weapons/" +
                                          ((NamePropertyData) propertyDatas).Value.Value.Value);
                asset.AddNameReference(newPath);

                ((NamePropertyData)propertyDatas).Value = new FName(newPath); 
            }
        }

        public Material(Cli cli) : base(cli)
        {

        }
    }
}
