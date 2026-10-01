using System.ComponentModel.DataAnnotations;

namespace IdentityMail.Web.DTOs.UserDtos
{
    public class RegisterDto
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? UserName { get; set; }
        [Required(ErrorMessage ="Şifre alanı boş bırakılamaz.")]
        public string? Password { get; set; }
        [Required(ErrorMessage = "Şifre doğrulama alanı boş bırakılamaz.")]
        public string? ConfirmPassword { get; set; }
        

        
    }
}
