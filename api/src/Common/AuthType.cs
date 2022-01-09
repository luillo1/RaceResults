using System.Text.Json.Serialization;

namespace RaceResults.Common.Models
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum AuthType
    {
        RaceResults,
        WildApricot,
    }
}
