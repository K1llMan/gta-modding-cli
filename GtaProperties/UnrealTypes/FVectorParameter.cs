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
        public FMaterialParameterInfo ParameterInfo { get; set; }

        [JsonProperty]
        public FLinearColor ParameterValue { get; set; }

        [JsonProperty]
        public Guid ExpressionGUID { get; set; }

        public FVectorParameter()
        {

        }
    }
}
