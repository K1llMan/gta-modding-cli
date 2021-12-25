using System.Linq;

using UAssetAPI;
using UAssetAPI.UnrealTypes;

namespace GtaModdingCli.Common
{
    public static class AssetUtils
    {
        #region Aux functions



        #endregion Aux functions

        #region Main functions

        public static void ChangeNameReference(UAsset asset, string oldName, string newName)
        {
            FString[] values = asset.GetNameMapIndexList()
                .Where(n => n.Value == oldName)
                .ToArray();

            foreach (FString value in values)
            {
                int reference = asset.SearchNameReference(value);
                asset.SetNameReference(reference, FString.FromString(newName));
            }
        }

        public static FPackageIndex AddImport(UAsset asset, string path, string type)
        {
            string name = path.Split("/").Last();

            asset.AddNameReference(FString.FromString(path));
            FPackageIndex pathIndex = asset.AddImport(new Import("/Script/CoreUObject", "Package", new FPackageIndex(), path));

            asset.AddNameReference(FString.FromString(type));
            asset.AddNameReference(FString.FromString(name));
            return asset.AddImport(new Import("/Script/Engine", type, pathIndex, name));
        }

        public static void ReplaceImport(UAsset asset, Import importName, string newObject)
        {
            Import importPath = importName.OuterIndex.ToImport(asset);

            ChangeNameReference(asset, importName.ObjectName.Value.Value, newObject.Split('/').Last());
            ChangeNameReference(asset, importPath.ObjectName.Value.Value, newObject);

            importName.ObjectName = FName.FromString(newObject.Split('/').Last());
            importPath.ObjectName = FName.FromString(newObject);
        }

        #endregion Main functions
    }
}
