using System;

using Newtonsoft.Json;

using UAssetAPI.UnrealTypes;

namespace GtaProperties.UnrealTypes
{
    /// <summary>
    /// Material texture parameter
    /// </summary>
    public class FTextureParameter
    {
        [JsonProperty]
        public FMaterialParameterInfo Info { get; set; }

        [JsonProperty]
        public FPackageIndex Value { get; set; }

        [JsonProperty]
        public Guid Guid { get; set; }

        public FTextureParameter()
        {

        }
    }
}
