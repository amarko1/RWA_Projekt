using System.ComponentModel.DataAnnotations;

namespace DATA_LAYER.JWTModels
{
    public class UserRegisterRequest
    {
        [Required, StringLength(50, MinimumLength = 6)]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string Email { get; set; }
        public string Phone { get; set; }
        [Required]
        public string Role { get; set; }
    }
}
