using System;
using System.IO;
using System.Linq;

using GtaModdingCli.Common;
using GtaModdingCli.Extensions;

using UAssetAPI;

namespace GtaModdingCli.Commands
{
    [CliCommand("Replace texture exports in material file.",
        "Usage: material [uasset] [search-pattern] [replace-pattern]\n" +
        "uasset: Path to material asset.n" +
        "search-pattern: Regular expression to search string in path.\n" +
        "  Example: (.*)(T_)(.+?)(_all)?(_.+) - search matches in string and set it in group by number.\n" +
        "replace-pattern: Replacement pattern. Can include special replacement {pack} - uasset name.\n" +
        "  Example: /Game/Weapons/{pack}/T_{pack}$5 - place all textures into /Game/Weapons/<uasset name>/T_<uasset name><texture type>, $5 - group with type from [search-pattern]",
        "material"
    )]
    public class Material : AbstractCliCommand
    {
        public override void Execute(string[] args)
        {
            string uassetName = args[0];

            if (!File.Exists(uassetName))
                throw new FileNotFoundException($"File \"{uassetName}\" not found");

            UAsset asset = new(args[0], UE4Version.VER_UE4_26);
            // Imported textures
            Import[] materialImport = asset.Imports
                .Where(i => i.ClassName.Value.Value == "Texture2D")
                .ToArray();

            string correctName = Path.GetFileNameWithoutExtension(uassetName)
                .Replace("MI_", string.Empty);

            Console.WriteLine("Replacements:");
            foreach (Import import in materialImport)
            {
                Import texture = import.OuterIndex.ToImport(asset);
                if (texture.ObjectName.Value.Value.Contains("Engine"))
                    continue;

                string replacePattern = args[2].ReplaceRegex("{pack}", correctName);
                string path = texture.ObjectName.Value.Value;
                string replacement = path.ReplaceRegex(args[1], replacePattern);
                Console.WriteLine($"{path} => {replacement}");

                // Update path import
                int reference = asset.SearchNameReference(texture.ObjectName.Value);
                FString newPath = FString.FromString(replacement);
                asset.SetNameReference(reference, newPath);
                texture.ObjectName.Value = newPath;

                // Update texture name
                reference = asset.SearchNameReference(import.ObjectName.Value);
                FString newName = FString.FromString(replacement.Split("/").Last());
                asset.SetNameReference(reference, newName);
                import.ObjectName.Value = newName;
            }

            string directory = Path.Combine(Path.GetDirectoryName(uassetName), "updated");
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);
            asset.Write(Path.Combine(directory, Path.GetFileName(uassetName)));
        }

        public Material(Cli cli) : base(cli)
        {

        }
    }
}
