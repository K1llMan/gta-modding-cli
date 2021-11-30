using GtaProperties.UnrealTypes;

using UAssetAPI;
using UAssetAPI.DataAccess;
using UAssetAPI.PropertyTypes;
using UAssetAPI.UnrealTypes;

namespace GtaProperties.PropertyTypes
{
    /// <summary>
    /// Describes MaterialTextureInfo
    /// </summary>
    public class MaterialTextureInfoPropertyData : PropertyData<FMaterialTextureInfo>
    {
        public MaterialTextureInfoPropertyData(FName name) : base(name)
        {

        }

        public MaterialTextureInfoPropertyData()
        {

        }

        private static readonly FName CurrentPropertyType = new("MaterialTextureInfo");
        public override bool HasCustomStructSerialization => true;
        public override FName PropertyType => CurrentPropertyType;

        public override void Read(AssetBinaryReader reader, bool includeHeader, long leng1, long leng2 = 0)
        {
            if (includeHeader)
            {
                reader.ReadByte();
            }

            FMaterialTextureInfo data = new()
            {
                SamplingScale = reader.ReadSingle(),
                UVChannelIndex = reader.ReadInt32(),
                TextureName = reader.ReadFName()
            };

            Value = data;
        }

        public override int Write(AssetBinaryWriter writer, bool includeHeader)
        {
            if (includeHeader)
            {
                writer.Write((byte)0);
            }

            int here = (int)writer.BaseStream.Position;

            writer.Write(Value.SamplingScale);
            writer.Write(Value.UVChannelIndex);
            writer.Write(Value.TextureName);

            return (int)writer.BaseStream.Position - here;
        }

        public override string ToString()
        {
            return Value.TextureName.Value.Value;
        }

        public override void FromString(string[] d, UAsset asset)
        {

        }
    }
}