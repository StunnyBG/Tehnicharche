using System.Text.Json.Serialization;

namespace Tehnicharche.Data.Seeding.DTOs
{
    internal class CategoryDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; } = null!;
    }
}
