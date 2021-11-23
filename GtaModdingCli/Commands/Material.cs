using GtaModdingCli.Commands.Classes;
using GtaModdingCli.Common;

namespace GtaModdingCli.Commands
{
    [CliCommand("Material editing commands.",
        "Usage: material [command] [command-args]+\n" +
        "command: See command list below." +
        "  empty [uasset] [out-path]\n" +
        "    Creates empty material from existing.\n" +
        "    uasset: Path to material asset.\n" +
        "    out-path: Path to output file.\n" +

        "  create [material-info-json] [out-path]\n" +
        "    Creates material with settings from json.\n" +
        "    material-info-json: Path to json material definition.\n" +
        "    out-path: Path to output directory.\n" +

        "  update [uasset] [material-name] [new-material-path]\n" +
        "    Creates material with settings from json.\n" +
        "    uasset: Path to material asset.\n" +
        "    material-name: Old material name to replace.\n" +
        "    new-material-path: New material path inside pak.\n" +

        "  replace [uasset] [search-pattern] [replace-pattern]\n" +
        "    Replace existing texture name to new by pattern.\n" +
        "    uasset: Path to material asset.\n" +
        "    search-pattern: Regular expression to search string in path.\n" +
        "      Example: (.*)(T_)(.+?)(_all)?(_.+) - search matches in string and set it in group by number.\n" +
        "    replace-pattern: Replacement pattern. Can include special replacement {pack} - uasset name.\n" +
        "      Example: /Game/Weapons/{pack}/T_{pack}$5 - place all textures into /Game/Weapons/[uasset name]/T_[uasset name][texture type], $5 - group with type from [search-pattern]",
        "material"
    )]
    public class Material : AbstractCliCommand
    {
        public override void Execute(string[] args)
        {
            MaterialEditor editor = new();

            switch (args[0])
            {
                case "empty":
                {
                    editor.GenerateEmptyMaterial(args[1], args[2]);
                    break;
                }

                case "create":
                {
                    editor.CreateMaterial(args[1], args[2]);
                    break;
                }

                case "update":
                {
                    editor.UpdateMaterialReference(args[1], args[2], args[3]);
                    break;
                }

                case "replace":
                {
                    editor.ReplaceFileNames(args[1], args[2], args[3]);
                    break;
                }
            }
        }

        public Material(Cli cli) : base(cli)
        {

        }
    }
}
