using System.ComponentModel.DataAnnotations;

namespace ApiEcommerce.Configurations
{
    public class JwtSettings
    {
        public const string Seccion = "JwtSettings";

        [Required]
        public required string Issuer { get; set; }
        [Required]
        public required string Audience { get; set; }
        [Range(1, 1440)]
        public required int ExpirationInMinutes { get; set; }
        [Required]
        public required string SecretKey { get; set; }
    }
}
