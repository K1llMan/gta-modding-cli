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

            /*
            FGTAModelData data = new FGTAModelData {
                ModelName = reader.ReadFString(),
                Index = reader.ReadInt32(),
            };

            FSoftObjectProperty asset = new FSoftObjectProperty();
            asset.Read(reader);

            data.Asset = asset;

            FSoftClassProperty blueprint = new FSoftClassProperty();
            blueprint.Read(reader);

            data.Blueprint = blueprint;

            short sh = reader.ReadInt16();

            int size = reader.ReadInt32();
            data.FXDatas = new FDFF2DEffect[size];
            for (int i = 0; i < data.FXDatas.Length; i++)
            {
                FDFF2DEffect effect = new FDFF2DEffect {
                    nodeName = reader.ReadFString()
                };
            }

            size = reader.ReadInt32();
            data.CollisionData = reader.ReadChars(size);

            size = reader.ReadInt32();
            data.Frames = new FDFFFrameData[size];
            for (int i = 0; i < data.Frames.Length; i++)
            {

            }
            
            data.ExtraStreamDistance = reader.ReadSingle();
            data.bClassReferenceIsUnique = reader.ReadBoolean();
            data.bHasFrameRotations = reader.ReadBoolean();
            data.bCanCollapseMesh = reader.ReadBoolean();
            reader.ReadByte();
            */

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