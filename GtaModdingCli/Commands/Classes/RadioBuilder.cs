using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using UAssetAPI;
using UAssetAPI.ExportTypes;
using UAssetAPI.PropertyTypes;
using UAssetAPI.PropertyTypes.Simple;
using UAssetAPI.PropertyTypes.Struct;
using UAssetAPI.UnrealTypes;
using UAssetAPI.UnrealTypes.Enums;

namespace GtaModdingCli.Commands.Classes
{
    public class RadioBuilder
    {
        #region Fields

        private UE4Version version = UE4Version.VER_UE4_26;
        private UAsset asset;
        private NormalExport player;

        #endregion Fields

        #region Aux functions

        private PropertyData GetTrackStruct(string path, string location, ref float startTime)
        {
            try
            {
                UAsset trackAsset = new(path, version);
                NormalExport track = (NormalExport)trackAsset.Exports.First();
                float duration = ((FloatPropertyData) track.Data.First(p => p.Name.Value.Value == "Duration")).Value;
                uint totalSamples = Convert.ToUInt32(((FloatPropertyData) track.Data.First(p => p.Name.Value.Value == "TotalSamples")).Value);

                string fileName = Path.GetFileNameWithoutExtension(path);
                FName pathName = new($"{location.TrimEnd('/')}/{fileName}.{fileName}");
                asset.AddNameReference(pathName.Value);

                StructPropertyData data = new()
                {
                    Name = new FName("SoundWaveAssets"),
                    StructType = new FName("CachedSoundWaveAsset"),
                    Value = new List<PropertyData> {
                        new FloatPropertyData { Name = new FName("Duration"), Value = duration },
                        new FloatPropertyData { Name = new FName("ActualStartTime"), Value = startTime },
                        new UInt32PropertyData { Name = new FName("TotalNumFrames"), Value = totalSamples },
                        new SoftObjectPropertyData {
                            Name = new FName("SoundAsset"),
                            Value = pathName
                        }
                    }
                };

                startTime += duration;

                return data;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Wrong asset file: {path}");
                throw;
            }
        }

        #endregion Aux functions

        #region Main functions

        /// <summary>
        /// Radio asset loading
        /// </summary>
        /// <param name="uasset">Asset filename</param>
        public void Load(string uasset)
        {
            if (!File.Exists(uasset))
                throw new FileNotFoundException($"File \"{uasset}\" not found");

            asset = new UAsset(uasset, version);
            Export export = asset.Exports.First(e => e.ObjectName.Value.Value == "SoundNodeProceduralWavePlayer");

            player = (NormalExport) export ?? throw new Exception("Not radio asset");
        }

        /// <summary>
        /// Track list adding
        /// </summary>
        /// <param name="path">Path to track assets</param>
        /// <param name="location">Location inside pack</param>
        public void AddTracks(string path, string location)
        {
            string[] files = Directory.GetFiles(path, "*.uasset");

            PropertyData property = player.Data.First(p => p.Name.Value.Value == "SoundWaveAssets");
            if (property is ArrayPropertyData assets)
            {
                float startTime = 0;

                assets.Value = files.Select(f => GetTrackStruct(f, location, ref startTime)).ToArray();
            }
        }

        /// <summary>
        /// Save radio asset
        /// </summary>
        /// <param name="path">Filename</param>
        public void Save(string path)
        {
            asset.Write(path);
        }

        #endregion Main functions
    }
}
