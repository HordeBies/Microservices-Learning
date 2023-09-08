using System.ComponentModel.DataAnnotations;

namespace IdentityServerHost.Quickstart.UI
{
    public class RegisterInputModel
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }        
        [Required]
        [Compare("Password", ErrorMessage = "Passwords don't match.")]
        public string ConfirmPassword { get; set; }
        public string GivenName { get; set; }
        public string FamilyName { get; set; }
        public string Website { get; set; }
        [Required]
        public string Role { get; set; }
        public string ReturnUrl { get; set; }
        
    }
}
