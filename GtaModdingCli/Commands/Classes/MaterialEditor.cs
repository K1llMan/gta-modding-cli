using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using GtaModdingCli.Common;
using GtaModdingCli.Extensions;

using GtaProperties.PropertyTypes;
using GtaProperties.UnrealTypes;
using GtaProperties.UnrealTypes.Enums;

using Newtonsoft.Json.Linq;

using UAssetAPI;
using UAssetAPI.ExportTypes;
using UAssetAPI.PropertyTypes;
using UAssetAPI.PropertyTypes.Simple;
using UAssetAPI.PropertyTypes.Struct;
using UAssetAPI.UnrealTypes;
using UAssetAPI.UnrealTypes.Enums;

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

        private void ChangeNameReference(UAsset asset, string oldName, string newName)
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

        private void ChangeExportName(Export export, string path)
        {
            string exportName = export.ObjectName.Value.Value;

            FString[] values = export.Asset.GetNameMapIndexList()
                .Where(n => n.Value.Contains(exportName))
                .ToArray();

            foreach (FString value in values)
            {
                string replace = value.Value == exportName
                    ? GetObjectName(path)
                    : path;

                ChangeNameReference(export.Asset, value.Value, replace);
            }

            export.ObjectName = FName.FromString(GetObjectName(path));
        }

        private void ChangeParentMaterial(NormalExport material, string parentStr)
        {
            if (string.IsNullOrEmpty(parentStr))
                return;

            UAsset asset = material.Asset;

            ObjectPropertyData parent = (ObjectPropertyData) material.Data
                .FirstOrDefault(p => p.Name.Value.Value == "Parent");

            if (parent == null)
                return;

            AssetUtils.ReplaceImport(asset, parent.ToImport(asset), parentStr);
        }

        private Import[] GetTextureImports(UAsset asset)
        {
            return asset.Imports
                .Where(i => i.ClassName.Value.Value == "Texture2D")
                .ToArray();
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

        private string ReplaceSubs(string str, string replace)
        {
            return str.Replace("{asset}", replace);
        }

        #region Properties setting

        private void SetSubsurfaceProfile(NormalExport material, string path)
        {
            if (string.IsNullOrEmpty(path))
                return;

            UAsset asset = material.Asset;
            ObjectPropertyData subsurface = (ObjectPropertyData)material.Data
                .FirstOrDefault(p => p.Name.Value.Value == "SubsurfaceProfile");

            if (subsurface == null)
            {
                subsurface = new ObjectPropertyData(FName.FromString("SubsurfaceProfile", asset)) {
                    Value = AssetUtils.AddImport(asset, path, "SubsurfaceProfile")
                };

                material.Data.Add(subsurface);
            }
            else
            {
                AssetUtils.ReplaceImport(asset, subsurface.Value.ToImport(asset), path);
            }
        }

        private void SetOverrides(NormalExport material, JObject overrideList)
        {
            UAsset asset = material.Asset;

            // Adding type strings
            string[] propertyTypes = { "FloatProperty", "BoolProperty", "ByteProperty" };
            foreach (string type in propertyTypes)
                asset.AddNameReference(FString.FromString(type));

            List<PropertyData> overrideParameters = new();
            foreach (JProperty property in overrideList.Properties())
            {
                switch (property.Name)
                {
                    case "OpacityMaskClipValue":
                    {
                        overrideParameters.Add(new FloatPropertyData {
                            Name = FName.FromString(property.Name, asset), 
                            Value = property.Value.Value<float>()
                        });
                        break;
                    }

                    case "TwoSided":
                    {
                        overrideParameters.Add(new BoolPropertyData {
                            Name = FName.FromString(property.Name, asset), 
                            Value = property.Value.Value<bool>()
                        });
                        break;
                    }

                    case "BlendMode":
                    {
                        int typeRef = asset.AddNameReference(FString.FromString(nameof(EBlendMode)));
                        int valueRef = asset.AddNameReference(FString.FromString(property.Value.Value<string>()));

                        overrideParameters.Add(new BytePropertyData {
                            ByteType = BytePropertyType.Long,
                            EnumType = typeRef,
                            Name = FName.FromString(property.Name, asset), 
                            Value = valueRef
                        });
                        break;
                    }

                    case "ShadingModel":
                    {
                        int typeRef = asset.AddNameReference(FString.FromString(nameof(EMaterialShadingModel)));
                        int valueRef = asset.AddNameReference(FString.FromString(property.Value.Value<string>()));

                        overrideParameters.Add(new BytePropertyData {
                            ByteType = BytePropertyType.Long,
                            EnumType = typeRef,
                            Name = FName.FromString(property.Name, asset),
                            Value = valueRef
                        });
                        break;
                    }
                }
            }

            GetProperty<StructPropertyData>(material, "BasePropertyOverrides")
                .Value = overrideParameters;
        }

        private void SetScalars(NormalExport material, JObject scalarList)
        {
            List<(string Name, float Value)> scalars = scalarList.Properties()
                .Select(p => (p.Name, p.Value.Value<float>()))
                .ToList();

            UAsset asset = material.Asset;

            List<StructPropertyData> scalarParameters = new();
            foreach ((string Name, float Value) scalar in scalars)
            {
                scalarParameters.Add(new StructPropertyData(FName.FromString("ScalarParameterValues"))
                {
                    StructType = FName.FromString("ScalarParameterValue"),
                    Value = new List<PropertyData> {
                        new ScalarParameterPropertyData(FName.FromString("ScalarParameterValues")) {
                            Value = new FScalarParameter {
                                ExpressionGUID = Guid.NewGuid(),
                                ParameterInfo = new FMaterialParameterInfo {
                                    Association = EMaterialParameterAssociation.GlobalParameter,
                                    Index = -1,
                                    Name = FName.FromString(scalar.Name, asset)
                                },
                                ParameterValue = scalar.Value
                            }
                        }
                    }
                });
            }

            GetProperty<ArrayPropertyData>(material, "ScalarParameterValues")
                .Value = scalarParameters.ToArray();
        }

        private void SetVectors(NormalExport material, JObject vectorList)
        {
            List<(string Name, FLinearColor Value)> vectors = vectorList.Properties()
                .Select(p => (p.Name, new FLinearColor {
                    R = p.Value["R"].Value<float>(),
                    G = p.Value["G"].Value<float>(),
                    B = p.Value["B"].Value<float>(),
                    A = p.Value["A"].Value<float>(),
                }))
                .ToList();

            UAsset asset = material.Asset;

            List<StructPropertyData> vectorParameters = new();
            foreach ((string Name, FLinearColor Value) vector in vectors)
            {
                vectorParameters.Add(new StructPropertyData(FName.FromString("VectorParameterValues"))
                {
                    StructType = FName.FromString("VectorParameterValue"),
                    Value = new List<PropertyData> {
                        new VectorParameterPropertyData(FName.FromString("VectorParameterValues")) {
                            Value = new FVectorParameter {
                                ExpressionGUID = Guid.NewGuid(),
                                ParameterInfo = new FMaterialParameterInfo {
                                    Association = EMaterialParameterAssociation.GlobalParameter,
                                    Index = -1,
                                    Name = FName.FromString(vector.Name, asset)
                                },
                                ParameterValue = vector.Value
                            }
                        }
                    }
                });
            }

            GetProperty<ArrayPropertyData>(material, "VectorParameterValues")
                .Value = vectorParameters.ToArray();
        }

        private void SetTextures(NormalExport material, JObject textureList, string name)
        {
            List<(string Type, string Path)> textures = textureList.Properties()
                .Select(p => (p.Name, p.Value.ToString()))
                .Where(t => !string.IsNullOrEmpty(t.Item2))
                .ToList();

            UAsset asset = material.Asset;

            List<StructPropertyData> textureParameters = new();
            List<StructPropertyData> textureStreaming = new();

            foreach ((string Type, string Path) texture in textures)
            {
                // Add new texture
                FPackageIndex textureIndex = AssetUtils.AddImport(asset, ReplaceSubs(texture.Path, name), "Texture2D");

                textureParameters.Add(new StructPropertyData(FName.FromString("TextureParameterValues")) {
                    StructType = FName.FromString("TextureParameterValue"),
                    Value = new List<PropertyData> {
                            new TextureParameterPropertyData(FName.FromString("TextureParameterValues")) {
                                Value = new FTextureParameter {
                                    ExpressionGUID = Guid.NewGuid(),
                                    ParameterInfo = new FMaterialParameterInfo {
                                        Association = EMaterialParameterAssociation.GlobalParameter,
                                        Index = -1,
                                        Name = FName.FromString(texture.Type, asset)
                                    },
                                    ParameterValue = textureIndex
                                }
                            }
                        }
                });

                textureStreaming.Add(new StructPropertyData(FName.FromString("TextureStreamingData")) {
                    StructType = FName.FromString("MaterialTextureInfo"),
                    Value = new List<PropertyData> {
                            new MaterialTextureInfoPropertyData(FName.FromString("TextureStreamingData")) {
                                Value = new FMaterialTextureInfo {
                                    UVChannelIndex = 0,
                                    SamplingScale = 1,
                                    TextureName = FName.FromString(GetObjectName(ReplaceSubs(texture.Path, name)), asset)
                                }
                            }
                        }
                });
            }

            GetProperty<ArrayPropertyData>(material, "TextureParameterValues")
                .Value = textureParameters.ToArray();
            GetProperty<ArrayPropertyData>(material, "TextureStreamingData")
                .Value = textureStreaming.ToArray();
        }

        #endregion Properties setting

        private void SaveNewMaterial(string name, JObject settings, string assetFileName, string outputPath)
        {
            // Textures
            string materialPath = ReplaceSubs(settings["material"].ToString(), name);

            UAsset asset = new(assetFileName, UE4Version.VER_UE4_26);
            Export material = asset.Exports.FirstOrDefault(e => e.ObjectName.Value.Value.StartsWith("MI_"));

            if (material is NormalExport ne)
            {
                // Replace material name
                ChangeExportName(material, materialPath);

                // Change base material
                ChangeParentMaterial(ne, settings["parent"]?.ToString());

                SetSubsurfaceProfile(ne, settings["subsurfaceProfile"]?.ToString());
                SetOverrides(ne, (JObject) settings["overrides"]);
                SetScalars(ne, (JObject) settings["scalar"]);
                SetVectors(ne, (JObject) settings["vector"]);
                SetTextures(ne, (JObject)settings["textures"], name);
            }

            if (!Directory.Exists(outputPath))
                Directory.CreateDirectory(outputPath);

            asset.Write(Path.Combine(outputPath, $"MI_{name}.uasset"));
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

            ChangeExportName(material, outputPath);

            // TODO: asset cleanup
            /*
            if (material is NormalExport ne)
            {
                // Clean properties
                ArrayPropertyData property = GetProperty<ArrayPropertyData>(ne, "TextureParameterValues");
                if (property == null)
                    throw new Exception("Cannot find texture parameters");

                // Clean imports
                foreach (PropertyData data in property.Value)
                {
                    FTextureParameter parameter = GetProperty<TextureParameterPropertyData>(data, "TextureParameterValues").Value;

                    // Link and texture

                    Import textureName = parameter.Value.ToImport(asset);
                    Import texturePath = textureName.OuterIndex.ToImport(asset);

                    textureName.ClassPackage = FName.FromString("/Script/CoreUObject");
                    textureName.ClassName = FName.FromString("Package");
                    textureName.OuterIndex = new FPackageIndex();
                    textureName.ObjectName = FName.FromString("/Engine/EngineMaterials/Good64x64TilingNoiseHighFreq");

                    texturePath.ClassPackage = FName.FromString("/Script/CoreUObject");
                    texturePath.ClassName = FName.FromString("Package");
                    texturePath.OuterIndex = new FPackageIndex();
                    texturePath.ObjectName = FName.FromString("/Engine/EngineMaterials/Good64x64TilingNoiseHighFreq");
                }

                property.Value = new PropertyData[] { };
                property = GetProperty<ArrayPropertyData>(ne, "TextureStreamingData");
                property.Value = new PropertyData[] { };
            }
            */

            asset.Write(outputPath);
        }

        /// <summary>
        /// Create material with texture properties
        /// </summary>
        /// <param name="fileName">Path to material definition json</param>
        /// <param name="outputPath">Output path</param>
        public void CreateMaterial(string fileName, string outputPath)
        {
            string baseAssetFileName = Path.Combine(AppContext.BaseDirectory, "sources", "MI_empty.uasset");
            if (!File.Exists(baseAssetFileName))
                throw new Exception("\"MI_empty.uasset\" does not exist in \"sources\" directory");

            JObject settings = JObject.Parse(File.ReadAllText(fileName));

            string[] names = settings["names"]
                .Select(n => n.ToString())
                .ToArray();

            foreach (string name in names)
                SaveNewMaterial(name, settings, baseAssetFileName, outputPath);
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
                Import matNameImport = asset.Imports.FirstOrDefault(i => i.ObjectName.Value.Value == materialName);
                if (matNameImport == null)
                    throw new Exception($"Material \"{materialName}\" not found");

                string newName = GetObjectName(newPath);
                
                // Update name import
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
