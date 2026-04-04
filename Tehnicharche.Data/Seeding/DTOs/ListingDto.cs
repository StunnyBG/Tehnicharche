using System.Text.Json.Serialization;

namespace Tehnicharche.Data.Seeding.DTOs
{
    internal class ListingDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; } = null!;

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("price")]
        public decimal Price { get; set; }

        [JsonPropertyName("categoryId")]
        public int CategoryId { get; set; }

        [JsonPropertyName("regionId")]
        public int RegionId { get; set; }

        [JsonPropertyName("cityId")]
        public int? CityId { get; set; }

        [JsonPropertyName("imageUrl")]
        public string? ImageUrl { get; set; }

        [JsonPropertyName("creatorId")]
        public string CreatorId { get; set; } = null!;

        [JsonPropertyName("createdAt")]
        public DateTime CreatedAt { get; set; }
    }
}
