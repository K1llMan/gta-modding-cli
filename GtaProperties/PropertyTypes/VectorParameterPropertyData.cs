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
    /// Describes VectorParameterValue
    /// </summary>
    public class VectorParameterPropertyData : PropertyData<FVectorParameter>
    {
        public VectorParameterPropertyData(FName name) : base(name)
        {

        }

        public VectorParameterPropertyData()
        {

        }

        private static readonly FName CurrentPropertyType = new("VectorParameterValue");
        public override bool HasCustomStructSerialization => true;
        public override FName PropertyType => CurrentPropertyType;

        public override void Read(AssetBinaryReader reader, bool includeHeader, long leng1, long leng2 = 0)
        {
            if (includeHeader)
            {
                reader.ReadByte();
            }

            FName name = reader.ReadFName();
            if (name.Value.Value == "None")
            {
                Value = null;
                return;
            }

            FVectorParameter data = new()
            {
                ParameterInfo = new FMaterialParameterInfo {
                    Name = name,
                    Association = (EMaterialParameterAssociation)reader.ReadByte(),
                    Index = reader.ReadInt32(),
                },
                ParameterValue = new FLinearColor { 
                    R = reader.ReadSingle(),
                    G = reader.ReadSingle(),
                    B = reader.ReadSingle(),
                    A = reader.ReadSingle(),
                },
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

            if (Value == null)
            {
                writer.Write(FName.FromString("None"));
            }
            else
            {
                writer.Write(Value.ParameterInfo.Name);
                writer.Write((byte)Value.ParameterInfo.Association);
                writer.Write(Value.ParameterInfo.Index);
                writer.Write(Value.ParameterValue.R);
                writer.Write(Value.ParameterValue.G);
                writer.Write(Value.ParameterValue.B);
                writer.Write(Value.ParameterValue.A);
                writer.Write(Value.ExpressionGUID.ToByteArray());
            }

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