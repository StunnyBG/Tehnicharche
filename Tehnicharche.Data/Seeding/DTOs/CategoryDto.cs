using System.Text.Json.Serialization;
using static Tehnicharche.GCommon.ValidationConstants.Category;

namespace Tehnicharche.Data.Seeding.DTOs
{
    internal class CategoryDto
    {
        [JsonRequired]
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonRequired]
        [JsonPropertyName("name")]
        public string Name { get; set; } = null!;
    }
}