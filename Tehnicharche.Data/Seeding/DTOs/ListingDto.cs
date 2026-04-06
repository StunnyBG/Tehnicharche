using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Tehnicharche.GCommon.Attributes;
using static Tehnicharche.GCommon.ValidationConstants.Listing;
 

namespace Tehnicharche.Data.Seeding.DTOs
{
    internal class ListingDto
    {
        [JsonRequired]
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonRequired]
        [MaxLength(TitleMaxLength)]
        [JsonPropertyName("title")]
        public string Title { get; set; } = null!;

        [MaxLength(DescriptionMaxLength)]
        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonRequired]
        [Price]
        [JsonPropertyName("price")]
        public decimal Price { get; set; }

        [JsonRequired]
        [JsonPropertyName("categoryId")]
        public int CategoryId { get; set; }

        [JsonRequired]
        [JsonPropertyName("regionId")]
        public int RegionId { get; set; }

        [JsonPropertyName("cityId")]
        public int? CityId { get; set; }

        [Url]
        [JsonPropertyName("imageUrl")]
        public string? ImageUrl { get; set; }

        [JsonRequired]
        [JsonPropertyName("creatorId")]
        public Guid CreatorId { get; set; }

        [JsonRequired]
        [JsonPropertyName("createdAt")]
        public DateTime CreatedAt { get; set; }
    }
}
