using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Tehnicharche.Data.Seeding.DTOs
{
    internal class UserDto
    {
        [JsonRequired]
        [JsonPropertyName("id")]
        public Guid Id { get; set; }

        [MaxLength(256)]
        [JsonPropertyName("userName")]
        public string UserName { get; set; } = null!;

        [EmailAddress]
        [MaxLength(256)]
        [JsonPropertyName("email")]
        public string Email { get; set; } = null!;

        [Phone]
        [JsonPropertyName("phoneNumber")]
        public string? PhoneNumber { get; set; }

        [JsonRequired]
        [JsonPropertyName("password")]
        public string Password { get; set; } = null!;

        [JsonPropertyName("roles")]
        public List<string>? Roles { get; set; }
    }
}