using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using static Tehnicharche.GCommon.ValidationConstants.Region;

namespace Tehnicharche.Data.Seeding.DTOs
{
    internal class RegionDto
    {
        [JsonRequired]
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonRequired]
        [MaxLength(NameMaxLength)]
        [JsonPropertyName("name")]
        public string Name { get; set; } = null!;
    }
}