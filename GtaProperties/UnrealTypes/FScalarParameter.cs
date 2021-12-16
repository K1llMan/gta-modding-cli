using System;

using Newtonsoft.Json;

namespace GtaProperties.UnrealTypes
{
    /// <summary>
    /// Material scalar parameter
    /// </summary>
    public class FScalarParameter
    {
        [JsonProperty]
        public FMaterialParameterInfo ParameterInfo { get; set; }

        [JsonProperty]
        public float ParameterValue { get; set; }

        [JsonProperty]
        public Guid ExpressionGUID { get; set; }

        public FScalarParameter()
        {

        }
    }
}
