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
    public class ScalarParameterPropertyData : PropertyData<FScalarParameter>
    {
        public ScalarParameterPropertyData(FName name) : base(name)
        {

        }

        public ScalarParameterPropertyData()
        {

        }

        private static readonly FName CurrentPropertyType = new("ScalarParameterValue");
        public override bool HasCustomStructSerialization => true;
        public override FName PropertyType => CurrentPropertyType;

        public override void Read(AssetBinaryReader reader, bool includeHeader, long leng1, long leng2 = 0)
        {
            if (includeHeader)
            {
                reader.ReadByte();
            }

            FScalarParameter data = new()
            {
                ParameterInfo = new FMaterialParameterInfo {
                    Name = reader.ReadFName(),
                    Association = (EMaterialParameterAssociation)reader.ReadByte(),
                    Index = reader.ReadInt32(),
                },
                ParameterValue = reader.ReadSingle(),
                ExpressionGUID = new Guid(reader.ReadBytes(16))
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

            writer.Write(Value.ParameterInfo.Name);
            writer.Write((byte)Value.ParameterInfo.Association);
            writer.Write(Value.ParameterInfo.Index);
            writer.Write(Value.ParameterValue);
            writer.Write(Value.ExpressionGUID.ToByteArray());

            return (int)writer.BaseStream.Position - here;
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