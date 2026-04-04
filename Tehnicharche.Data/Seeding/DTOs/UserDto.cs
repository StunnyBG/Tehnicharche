using System.Text.Json.Serialization;

namespace Tehnicharche.Data.Seeding.DTOs
{
    internal class UserDto
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = null!;

        [JsonPropertyName("userName")]
        public string UserName { get; set; } = null!;

        [JsonPropertyName("email")]
        public string Email { get; set; } = null!;

        [JsonPropertyName("phoneNumber")]
        public string? PhoneNumber { get; set; }

        [JsonPropertyName("password")]
        public string Password { get; set; } = null!;

        [JsonPropertyName("roles")]
        public List<string>? Roles { get; set; }
    }
}
