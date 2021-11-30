using System;

using GtaProperties.UnrealTypes;

using UAssetAPI;
using UAssetAPI.DataAccess;
using UAssetAPI.PropertyTypes;
using UAssetAPI.UnrealTypes;
using UAssetAPI.UnrealTypes.Enums;

namespace GtaProperties.PropertyTypes
{
    /// <summary>
    /// Describes ScalarParameterValue
    /// </summary>
    public class TextureParameterPropertyData : PropertyData<FTextureParameter>
    {
        public TextureParameterPropertyData(FName name) : base(name)
        {

        }

        public TextureParameterPropertyData()
        {

        }

        private static readonly FName CurrentPropertyType = new("TextureParameterValue");
        public override bool HasCustomStructSerialization => true;
        public override FName PropertyType => CurrentPropertyType;

        public override void Read(AssetBinaryReader reader, bool includeHeader, long leng1, long leng2 = 0)
        {
            if (includeHeader)
            {
                reader.ReadByte();
            }

            FTextureParameter data = new()
            {
                Info = new FMaterialParameterInfo {
                    Name = reader.ReadFName(),
                    Association = (EMaterialParameterAssociation)reader.ReadByte(),
                    Index = reader.ReadInt32(),
                },
                Value = new FPackageIndex(reader.ReadInt32()),
                Guid = new Guid(reader.ReadBytes(16))
            };

            Value = data;
        }

        public override int Write(AssetBinaryWriter writer, bool includeHeader)
        {
            if (includeHeader)
            {
                writer.Write((byte)0);
            }

            writer.Write(Value.Info.Name);
            writer.Write((byte)Value.Info.Association);
            writer.Write(Value.Info.Index);
            writer.Write(Value.Value.Index);
            writer.Write(Value.Guid.ToByteArray());

            return sizeof(short);
        }

        public override string ToString()
        {
            return Convert.ToString(Value);
        }

        public override void FromString(string[] d, UAsset asset)
        {

        }
    }
}