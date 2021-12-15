using System;

using Newtonsoft.Json;

namespace GtaProperties.UnrealTypes
{
    /// <summary>
    /// Material scalar parameter
    /// </summary>
    public class FVectorParameter
    {
        [JsonProperty]
        public FMaterialParameterInfo Info { get; set; }

        [JsonProperty]
        public FLinearColor Value { get; set; }

        [JsonProperty]
        public Guid Guid { get; set; }

        public FVectorParameter()
        {

        }
    }
}
