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
        public FMaterialParameterInfo ParameterInfo { get; set; }

        [JsonProperty]
        public FPackageIndex ParameterValue { get; set; }

        [JsonProperty]
        public Guid ExpressionGUID { get; set; }

        public FTextureParameter()
        {

        }
    }
}
