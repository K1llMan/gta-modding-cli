using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GtaModdingCli.Common;

using UAssetAPI;
using UAssetAPI.ExportTypes;
using UAssetAPI.PropertyTypes;
using UAssetAPI.PropertyTypes.Simple;
using UAssetAPI.UnrealTypes;
using UAssetAPI.UnrealTypes.Enums;

namespace GtaModdingCli.Commands.Classes
{
    public class CarEditor
    {
        #region Aux functions

        private void AddWheelProperty(NormalExport export, string path)
        {
            UAsset asset = export.Asset;

            ObjectPropertyData wheelMesh = (ObjectPropertyData) export.Data
                .FirstOrDefault(p => p.Name.Value.Value == "WheelMesh");

            if (wheelMesh == null)
            {
                wheelMesh = new ObjectPropertyData(FName.FromString("WheelMesh", asset));

                FPackageIndex index = asset.AddImport(new("Package", "/Script/CoreUObject", new FPackageIndex(), path));
                asset.AddImport(new("/Script/Engine", "StaticMesh", index, path.Split('/').Last()));
                export.Data.Add(wheelMesh);
            }
            else
            {
                AssetUtils.ReplaceImport(asset, wheelMesh.ToImport(asset), path);
            }
        }

        #endregion Aux functions

        #region Main functions

        public void AddWheel(string assetPath, string wheelPath)
        {
            UAsset asset = new(assetPath, UE4Version.VER_UE4_26);
            Export mainExport = asset.Exports.First(e => e.OuterIndex.Index == 0);


            Export vehicleData = asset.Exports
                .FirstOrDefault(e => e.GetExportClassType().Value.Value == "GTAVehicleUserData");

            

            if (vehicleData == null)
            {
                vehicleData = new() {
                    Asset = asset,
                    //OuterIndex = new FPackageIndex(mainExport.)


                    ObjectName = FName.FromString("GTAVehicleUserData", asset)
                };



                asset.Exports.Add(vehicleData);
            }

            if (vehicleData is NormalExport ne)
                AddWheelProperty(ne, wheelPath);

            asset.Write(assetPath);
        }

        #endregion Main functions
    }
}
