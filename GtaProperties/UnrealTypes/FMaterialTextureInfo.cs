using Newtonsoft.Json;

using UAssetAPI.UnrealTypes;

namespace GtaProperties.UnrealTypes
{
    public class FMaterialTextureInfo
    {
        [JsonProperty]
        public float SamplingScale { get; set; }

        [JsonProperty]
        public int UVChannelIndex { get; set; }

        [JsonProperty]
        public FName TextureName { get; set; }

        public FMaterialTextureInfo()
        {

        }
    }
}
