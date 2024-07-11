using System.Text.Json.Serialization;

namespace ShikimoriSharp.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum Duration
    {
        /// <summary>
        /// Duration is less than 10 minutes.
        /// </summary>
        S,
        /// <summary>
        /// Duration is less than 30 minutes.
        /// </summary>
        D,
        /// <summary>
        /// Duration is more than 30 minutes.
        /// </summary>
        F
    }
}