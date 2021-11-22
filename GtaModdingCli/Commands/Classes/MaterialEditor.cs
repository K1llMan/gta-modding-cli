using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using GtaModdingCli.Extensions;

using Newtonsoft.Json.Linq;

using UAssetAPI;
using UAssetAPI.PropertyTypes;
using UAssetAPI.StructTypes;

namespace GtaModdingCli.Commands.Classes
{
    public class MaterialEditor
    {
        #region Aux functions

        private string GetObjectName(string path)
        {
            return path.Split("/").Last();
        }

        private T GetProperty<T>(NormalExport export, string name) where T: PropertyData
        {
            return (T)export.Data.FirstOrDefault(p => p.Name.Value.Value == name);
        }

        private T GetProperty<T>(PropertyData property, string name) where T : PropertyData
        {
            if (property is StructPropertyData sp)
            {
                return (T) sp.Value.FirstOrDefault(p => p.Name.Value.Value == name);
            }

            return null;
        }

        private void ChangeExportName(Export export, string name)
        {
            int reference = export.Asset.SearchNameReference(export.ObjectName.Value);
            export.Asset.SetNameReference(reference, FString.FromString(name));

            export.ObjectName = FName.FromString(name);
        }

        private Import[] GetTextureImports(UAsset asset)
        {
            return asset.Imports
                .Where(i => i.ClassName.Value.Value == "Texture2D")
                .ToArray();
        }

        private FPackageIndex AddTextureImport(UAsset asset, string path)
        {
            string name = GetObjectName(path.Split("/").Last());

            asset.AddNameReference(FString.FromString(path));
            FPackageIndex pathIndex = asset.AddImport(new Import("/Script/CoreUObject", "Package", new FPackageIndex(), path));

            asset.AddNameReference(FString.FromString(name));
            return asset.AddImport(new Import("/Script/Engine", "Texture2D", pathIndex, name));
        }

        private void RenameImport(Import import, UAsset asset, string newName)
        {
            int reference = asset.SearchNameReference(import.ObjectName.Value);
            FString newPath = FString.FromString(newName);
            asset.SetNameReference(reference, newPath);
            import.ObjectName.Value = newPath;
        }

        private void WriteUpdated(UAsset asset)
        {
            string directory = Path.Combine(Path.GetDirectoryName(asset.FilePath), "updated");

            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);
            asset.Write(Path.Combine(directory, Path.GetFileName(asset.FilePath)));
        }

        #endregion Aux functions

        #region Main functions

        /// <summary>
        /// Generates empty material template from existing material
        /// </summary>
        /// <param name="uasset">Path to uasset</param>
        /// <param name="outputPath">Output path</param>
        public void GenerateEmptyMaterial(string uasset, string outputPath)
        {
            UAsset asset = new(uasset, UE4Version.VER_UE4_26);
            Export material = asset.Exports.FirstOrDefault(e => e.ObjectName.Value.Value.StartsWith("MI_"));
            if (material == null)
                throw new Exception("Cannot find material export");

            ChangeExportName(material, Path.GetFileNameWithoutExtension(outputPath));

            if (material is NormalExport ne)
            {
                // Clean properties
                ArrayPropertyData property = GetProperty<ArrayPropertyData>(ne, "TextureParameterValues");
                if (property == null)
                    throw new Exception("Cannot find texture parameters");

                property.Value = new PropertyData[] { };
                property = GetProperty<ArrayPropertyData>(ne, "TextureStreamingData");
                property.Value = new PropertyData[] { };

                // Clean imports
                foreach (Import import in GetTextureImports(asset))
                {
                    Import texture = import.OuterIndex.ToImport(asset);
                    if (texture.ObjectName.Value.Value.Contains("Engine"))
                        continue;

                    // Link and texture
                    asset.Imports.Remove(import);
                    asset.Imports.Remove(texture);
                }
            }

            asset.Write(outputPath);
        }

        /// <summary>
        /// Create material with texture properties
        /// </summary>
        /// <param name="fileName">Path to material definition json</param>
        /// <param name="outputPath">Output path</param>
        public void CreateMaterial(string fileName, string outputPath)
        {
            JObject settings = JObject.Parse(File.ReadAllText(fileName));
            
            string materialPath = settings["material"].ToString();
            JObject texList = (JObject) settings["textures"];

            List<(string Type, string Path)> textures = texList.Properties()
                .Select(p => (p.Name, p.Value.ToString()))
                .Where(t => !string.IsNullOrEmpty(t.Item2))
                .ToList();

            string assetFileName = Path.Combine(AppContext.BaseDirectory, "sources", "MI_empty.uasset");
            if (!File.Exists(assetFileName))
                throw new Exception("\"MI_empty.uasset\" does not exist in \"sources\" directory");

            UAsset asset = new UAsset(assetFileName, UE4Version.VER_UE4_26);
            Export material = asset.Exports.FirstOrDefault(e => e.ObjectName.Value.Value.StartsWith("MI_"));

            if (material is NormalExport ne)
            {
                // Replace material name
                ChangeExportName(material, GetObjectName(materialPath));

                List<TextureParameterPropertyData> textureParameters = new List<TextureParameterPropertyData>();
                List<MaterialTextureInfoPropertyData> textureStreaming = new List<MaterialTextureInfoPropertyData>();

                foreach ((string Type, string Path) texture in textures)
                {
                    // Add new texture
                    FPackageIndex textureIndex = AddTextureImport(asset, texture.Path);

                    textureParameters.Add(new TextureParameterPropertyData(FName.FromString("TextureParameterValues")) {
                        Value = new FTextureParameter {
                            Guid = new Guid(),
                            Info = new FMaterialParameterInfo {
                                Association = EMaterialParameterAssociation.GlobalParameter,
                                Index = -1,
                                Name = FName.FromString(texture.Type)
                            },
                            Value = textureIndex
                        }
                    });

                    textureStreaming.Add(new MaterialTextureInfoPropertyData(FName.FromString("TextureStreamingData")) {
                        Value = new FMaterialTextureInfo 
                        {
                            UVChannelIndex = 0,
                            SamplingScale = 1,
                            TextureName = FName.FromString(GetObjectName(texture.Path))
                        }
                    });
                }

                GetProperty<ArrayPropertyData>(ne, "TextureParameterValues")
                    .Value = textureParameters.ToArray();
                GetProperty<ArrayPropertyData>(ne, "TextureStreamingData")
                    .Value = textureStreaming.ToArray();
            }

            asset.Write(outputPath);
        }

        /// <summary>
        /// Update material reference inside model uasset
        /// </summary>
        /// <param name="uasset">Path to uasset</param>
        /// <param name="materialName">Path to material uasset inside .pak</param>
        /// <param name="newPath">Path to new material uasset inside .pak</param>
        public void UpdateMaterialReference(string uasset, string materialName, string newPath)
        {
            UAsset asset = new(uasset, UE4Version.VER_UE4_26);
            Export export = asset.Exports.FirstOrDefault(e => e.ObjectName.Value.Value == Path.GetFileNameWithoutExtension(uasset));
            if (export == null)
                throw new Exception("Cannot find model export");

            if (export is NormalExport ne)
            {
                ArrayPropertyData materials = GetProperty<ArrayPropertyData>(ne, "StaticMaterials");
                PropertyData material = materials.Value
                    .FirstOrDefault(p => GetProperty<NamePropertyData>(p, "MaterialSlotName").Value.Value.Value == materialName);
                if (material == null)
                    throw new Exception($"Material \"{materialName}\" not found");

                string newName = GetObjectName(newPath);
                asset.AddNameReference(FString.FromString(newName));
                GetProperty<NamePropertyData>(material, "MaterialSlotName").Value = FName.FromString(newName);
                
                // Update name import
                Import matNameImport = GetProperty<ObjectPropertyData>(material, "MaterialInterface").Value.ToImport(asset);
                RenameImport(matNameImport, asset, newName);

                // Update path import
                Import matPathImport = matNameImport.OuterIndex.ToImport(asset);
                RenameImport(matPathImport, asset, newPath);
            }

            WriteUpdated(asset);
        }

        /// <summary>
        /// Replace existing texture imports to new by patterns
        /// </summary>
        /// <param name="uasset">Path to uasset</param>
        /// <param name="search">Search pattern</param>
        /// <param name="replace">Replace pattern</param>
        public void ReplaceFileNames(string uasset, string search, string replace)
        {
            if (!File.Exists(uasset))
                throw new FileNotFoundException($"File \"{uasset}\" not found");

            UAsset asset = new(uasset, UE4Version.VER_UE4_26);
            string correctName = Path.GetFileNameWithoutExtension(uasset)
                .Replace("MI_", string.Empty);

            Console.WriteLine("Replacements:");

            // Imported textures
            foreach (Import import in GetTextureImports(asset))
            {
                Import texture = import.OuterIndex.ToImport(asset);
                if (texture.ObjectName.Value.Value.Contains("Engine"))
                    continue;

                string replacePattern = replace.ReplaceRegex("{pack}", correctName);
                string path = texture.ObjectName.Value.Value;
                string replacement = path.ReplaceRegex(search, replacePattern);
                Console.WriteLine($"{path} => {replacement}");

                // Update path import
                RenameImport(texture, asset, replacement);

                // Update texture name
                RenameImport(import, asset, GetObjectName(replacement));
            }

            WriteUpdated(asset);
        }

        #endregion Main functions
    }
}
