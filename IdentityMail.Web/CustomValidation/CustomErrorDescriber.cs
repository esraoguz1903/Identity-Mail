using Microsoft.AspNetCore.Identity;

namespace IdentityMail.Web.CustomValidation
{
    public class CustomErrorDescriber : IdentityErrorDescriber  //Default olarak identity den gelen hata mesajlarını override ederek özelleştirip Türkçeleştiriyoruz burada.
    {
        public override IdentityError PasswordRequiresDigit()
        {
            return new IdentityError
            {
                Code = "PasswordRequiresDigit",
                Description = "Şifre en az bir rakam içermelidir. (1-9)"
            };
        }

        public override IdentityError PasswordRequiresLower()
        {
            return new IdentityError
            {
                Code = "PasswordRequiresLower",
                Description = "Şifre en az bir küçük harf içermelidir. (a-z)"
            };
        }

        public override IdentityError PasswordRequiresUpper()
        {
            return new IdentityError
            {
                Code = "PasswordRequiresUpper",
                Description = "Şifre en az bir büyük harf içermelidir. (A-Z)"
            };
        }

        public override IdentityError PasswordRequiresNonAlphanumeric()
        {
            return new IdentityError
            {
                Code = "PasswordRequiresNonAlphanumeric",
                Description = "Şifre en az bir özel karakter içermelidir. (*,!,.,? vs)"
            };
        }

        public override IdentityError PasswordTooShort(int length)
        {
            return new IdentityError
            {
                Code = "PasswordTooShort",
                Description = $"Şifre en az {length} rakamdan oluşmalıdır."
            };
        }

        public override IdentityError DuplicateUserName(string userName)
        {
            return new IdentityError
            {
                Code = "DuplicateUserName",
                Description = $"{userName} kullanıcı adı daha önceden alınmış."
            };
        }
    }
}
