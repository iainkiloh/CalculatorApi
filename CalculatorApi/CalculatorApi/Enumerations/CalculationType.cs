using System.Text.Json.Serialization;

namespace CalculatorApi.Enumerations
{
    /// <summary>
    /// enumeration specifying the available math operations
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum CalculationType
    {
        Add,
        Multiply,
        Subtract
    }
}
