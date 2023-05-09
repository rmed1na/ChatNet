using System.Text.Json;

namespace ChatNet.Utils.Object
{
    public static class ObjectExtensions
    {
        /// <summary>
        /// Serialize an object to a json string
        /// </summary>
        /// <param name="obj">Object to serialize</param>
        /// <returns>Serialized representation of the object as a string</returns>
        public static string? ToJson(this object? obj)
            => obj is null ? null : JsonSerializer.Serialize(obj);
    }
}