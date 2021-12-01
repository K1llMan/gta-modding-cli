using UAssetAPI.DataAccess;
using UAssetAPI.UnrealTypes;

namespace GtaProperties.UnrealTypes
{
    public class FDFF2DEffect
    {
        public FString nodeName { get; set; }                                                  // 0x0000(0x0010) (Edit, ZeroConstructor, EditConst, HasGetValueTypeHash, NativeAccessSpecifierPublic)
        public char[] Data { get; set; }                                                      // 0x0010(0x0010) (Edit, ZeroConstructor, EditConst, NativeAccessSpecifierPublic)
    };

    // ScriptStruct GTABase.DFFFrameData
    // 0x0020
    public class FDFFFrameData_old
    {
        public FName BoneName { get; set; }                                                  // 0x0000(0x0008) (Edit, ZeroConstructor, IsPlainOldData, NoDestructor, HasGetValueTypeHash, NativeAccessSpecifierPublic)
        public int BoneTag { get; set; }                                                   // 0x0008(0x0004) (Edit, ZeroConstructor, IsPlainOldData, NoDestructor, HasGetValueTypeHash, NativeAccessSpecifierPublic)
        public char UnknownData_N8CX { get; set; }                                     // 0x000C(0x0004) MISSED OFFSET (FIX SPACE BETWEEN PREVIOUS PROPERTY)
        public FQuat BoneRotation { get; set; }                                             // 0x0010(0x0010) (Edit, EditConst, IsPlainOldData, NoDestructor, NativeAccessSpecifierPublic)
    };

	public class FGTAModelData_old
    {
		public FString ModelName { get; set; }						// 0x0000(0x0010) (Edit, ZeroConstructor, HasGetValueTypeHash, NativeAccessSpecifierPublic)
	    public int Index { get; set; }                                                // 0x0010(0x0004) (Edit, ZeroConstructor, EditConst, IsPlainOldData, NoDestructor, HasGetValueTypeHash, NativeAccessSpecifierPublic)
		public char UnknownData_ZIKJ { get; set; }                                     // 0x0014(0x0004) MISSED OFFSET (FIX SPACE BETWEEN PREVIOUS PROPERTY)
		public FSoftObjectProperty Asset { get; set; }                                               // 0x0014(0x0028) UNKNOWN PROPERTY: SoftObjectProperty
		public FSoftClassProperty Blueprint { get; set; }                                           // 0x0040(0x0028) UNKNOWN PROPERTY: SoftClassProperty
		public FDFF2DEffect[] FXDatas { get; set; }                                                   // 0x0068(0x0010) (Edit, ZeroConstructor, EditConst, NativeAccessSpecifierPublic)
        public char[] CollisionData { get; set; }                                             // 0x0078(0x0010) (Edit, ZeroConstructor, EditConst, NativeAccessSpecifierPublic)
        public FDFFFrameData[] Frames { get; set; }                                                    // 0x0088(0x0010) (Edit, ZeroConstructor, NativeAccessSpecifierPublic)
        public float ExtraStreamDistance { get; set; }                                       // 0x0098(0x0004) (Edit, ZeroConstructor, IsPlainOldData, NoDestructor, HasGetValueTypeHash, NativeAccessSpecifierPublic)
        public bool bClassReferenceIsUnique { get; set; }                                   // 0x009C(0x0001) (Edit, ZeroConstructor, Transient, EditConst, IsPlainOldData, NoDestructor, HasGetValueTypeHash, NativeAccessSpecifierPublic)
        public bool bHasFrameRotations { get; set; }                                        // 0x009D(0x0001) (Edit, ZeroConstructor, EditConst, IsPlainOldData, NoDestructor, HasGetValueTypeHash, NativeAccessSpecifierPublic)
        public bool bCanCollapseMesh { get; set; }                                          // 0x009E(0x0001) (Edit, ZeroConstructor, EditConst, IsPlainOldData, NoDestructor, HasGetValueTypeHash, NativeAccessSpecifierPublic)
        public char UnknownData_OW0H { get; set; }                                     // 0x009F(0x0001) MISSED OFFSET (PADDING)
    }



    public class FDFFFrameData
    {
        public FName BoneName { get; set; }
        public int BoneTag { get; set; }
        public FQuat BoneRotation { get; set; }

        public void Read(AssetBinaryReader reader)
        {
            BoneName = reader.ReadFName();
            BoneTag = reader.ReadInt32();
            BoneRotation = new FQuat(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
        }
    }

    public class FGTAModelData
    {
        public FString ModelName { get; set; }
        public int Index { get; set; }
        public FName Name { get; set; }
        public FVector Position { get; set; }
        public FDFFFrameData[] Frames { get; set; }
        public FName Blueprint { get; set; }
        public bool ClassReferenceIsUnique { get; set; }
        public bool HasFrameRotations { get; set; }
        public bool CanCollapseMesh { get; set; }

        public void Read(AssetBinaryReader reader)
        {
            reader.ReadByte();

            ModelName = reader.ReadFString();
            Index = reader.ReadInt32();
            Name = reader.ReadFName();
            Position = new FVector(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());

            int size = reader.ReadInt32();
            Frames = new FDFFFrameData[size];
            for (int i = 0; i < size; i++)
            {
                Frames[i] = new FDFFFrameData();
                Frames[i].Read(reader);
            }

            Blueprint = reader.ReadFName();
            ClassReferenceIsUnique = reader.ReadBoolean();
            HasFrameRotations = reader.ReadBoolean();
            CanCollapseMesh = reader.ReadBoolean();
            reader.ReadByte();
        }

    }
}
