using IdentityMail.Web.DTOs.UserPasswordDtos;
using IdentityMail.Web.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace IdentityMail.Web.Controllers
{
    public class PasswordController(UserManager<AppUser> _userManager) : Controller
    {
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordDto forgotPasswordDto)
        {
            //Validation kurallarına uyup uymadığını kontrol ediyoruz.
            if (!ModelState.IsValid)
            {
                return View();
            }

            var user = await _userManager.FindByEmailAsync(forgotPasswordDto.Email);
            if (user is null)
            {
                ViewBag.message = "Eğer sistemde kayıtlıysanız sıfırlama linki e-postanıza iletilmiştir.";
                return View();
            }

            //Token olşturuyoruz burada.(Aşağıda bu tokenı link haline getireceğiz.
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            //Yukarıda ürettiğimiz token ı link haline getiriyoruz ki kullanıcıyı şifre sıfırlama sayfasına yönlendirebilelim.
            var resetPasswordLink = Url.Action("ResetPassword", "Password", new
            {
                email = forgotPasswordDto.Email,
                token = token
            }, Request.Scheme);  //Eğer bunu yazmazsak oluşturduğumuz link yarım olur bunu alırsak o sitenin çalıştığı protokolü de otomatik olarak alır ve tam bir yol oluşturarak bir lin oluşturur.


            await SendResetEmailAsync(user.Email, resetPasswordLink);
            ViewBag.resetLinkMessage = "Şifre sıfırlama linki e-posta adresinize gönderildi.";
            return View();
        }

        //Şifre sıfırlama ekranı için
        public IActionResult ResetPassword(string email, string token)
        {
            //Bu kod bloğu olmazsa eğer tarayıcıdan kötü niyetli biri mi birinin epostasını giererek şifre sıfırlıyor yoksa gerçekten kişi şifresini unuttu da o mu sıfırlıyor anlaşılmaz. Identitynin ResetPasswordAsync() metdonunu çalıştırabilmesi için hangi hesaba işlem yaptığını bilmesi şart.
            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(email))
            {
                return RedirectToAction("Login", "Auth");
            }

            var model = new ResetPasswordDto { Email = email, Token = token }; //Şifreler dışarıdan girilecek. Girildikten sonra bütün dto lar değerlerle dolmuş olacak.
            return View(model);

        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordDto resetPasswordDto)
        {
            if (!ModelState.IsValid)
            {
                return View(resetPasswordDto);
            }

            var user = await _userManager.FindByEmailAsync(resetPasswordDto.Email);
            if (user is null)
            {
                return RedirectToAction("Login", "Auth");
            }

            var result = await _userManager.ResetPasswordAsync(user, resetPasswordDto.Token, resetPasswordDto.NewPassword);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            //AppUser sınıfında SecurityStamp adında kullanıcının oturumunun geçerliliğini temsil eden Guid bir değer mevcut. Kullanıcı sisteme giriş yaptığında bir securityStamp değeri oluşturur ve çereze de bu değer gelir sistem ara ara kendini kontrol eder ve tarayıcıdaki securitystamp ile vt deki aynıysa oturum sıkıntısız açık kalır. Ancak bir yerde oturumu açık unutsan ya da şifren çalınsa eğer bu değeri bu şekilde güncellersen vt deki ile tarayıcıdaki farklı değer olacağı için eski şifre ile açık olan bütün oturumlar geçersiz hale geliyor. Bu gibi güvenlik açıklarının önüne geçmek için kullanıyoruz.
            await _userManager.UpdateSecurityStampAsync(user);
            TempData["SuccessMessage"] = "Şifreniz başarıyla güncellendi. Yeni şifrenizle giriş yapabilirsiniz.";
            return RedirectToAction("Login", "Auth");
        }


        private async Task SendResetEmailAsync(string toEmail, string resetLink)
        {
            var smtpClient = new SmtpClient("sandbox.smtp.mailtrap.io")
            {
                Port = 2525,
                EnableSsl = true,
                Credentials = new NetworkCredential("2403da8a3ff82e", "410a329fc4ac59")
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress("destek@novaconnect.com", "Nova Connect Destek"),
                Subject = "Şifre Sıfırlama Talebi",
                Body = $@"
            <div style='font-family: Arial, sans-serif; padding: 20px;'>
                <h2>Şifre Sıfırlama Talebi</h2>
                <p>Hesabınızın şifresini sıfırlamak için aşağıdaki butona tıklayın:</p>
                <p>
                    <a href='{resetLink}' style='background-color: #68548d; color: white; padding: 10px 20px; text-decoration: none; border-radius: 5px; display: inline-block;'>
                        Şifremi Sıfırla
                    </a>
                </p>
                <p style='color: #777; font-size: 12px;'>Bu talebi siz yapmadıysanız bu e-postayı dikkate almayınız.</p>
            </div>",
                IsBodyHtml = true
            };

            //Bu satır şifre gönderme linkini hangi kullanıcı kullandıysa onun mail adresine gitmesi için var.
            mailMessage.To.Add(toEmail);
            await smtpClient.SendMailAsync(mailMessage);
        }
    }
}
