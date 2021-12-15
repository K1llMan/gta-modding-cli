using System;

using GtaProperties.UnrealTypes;

using UAssetAPI;
using UAssetAPI.DataAccess;
using UAssetAPI.PropertyTypes;
using UAssetAPI.UnrealTypes;

namespace GtaProperties.PropertyTypes
{
    /// <summary>
    /// Describes ScalarParameterValue
    /// </summary>
    public class GTAModelDataPropertyData : PropertyData<FGTAModelData>
    {
        public GTAModelDataPropertyData(FName name) : base(name)
        {

        }

        public GTAModelDataPropertyData()
        {

        }

        private static readonly FName CurrentPropertyType = new("GTAModelData");
        public override bool HasCustomStructSerialization => true;
        public override FName PropertyType => CurrentPropertyType;

        public override void Read(AssetBinaryReader reader, bool includeHeader, long leng1, long leng2 = 0)
        {
            if (includeHeader)
            {
                reader.ReadByte();
            }
            

            FGTAModelData data = new FGTAModelData();
            data.Read(reader);

            Value = data;
        }

        public override int Write(AssetBinaryWriter writer, bool includeHeader)
        {
            if (includeHeader)
            {
                writer.Write((byte)0);
            }


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