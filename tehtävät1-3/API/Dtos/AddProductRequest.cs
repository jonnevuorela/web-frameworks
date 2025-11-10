using System.Text.Json.Serialization;

namespace API.Dtos
{
    public class AddProductRequest
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;
    }
}
