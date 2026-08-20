using IdentityMail.Web.DTOs.UserDtos;
using IdentityMail.Web.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IdentityMail.Web.Controllers
{
    public class AuthController(UserManager<AppUser> _userManager,
                                SignInManager<AppUser> _signInManager) : Controller //primary constructor. DI yaparken constructor ve field oluşturmadan aynı işlemi yapıyor primary ctor.
    {

        //private readonly UserManager<AppUser> _userManager;
        //public AuthController(UserManager<AppUser> userManager)
        //{
        //    _userManager = userManager;
        //}

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterDto registerDto)
        {
            if(registerDto.Password != registerDto.ConfirmPassword)
            {
                ModelState.AddModelError(string.Empty, "Şifreler birbiri ile uyumlu değil.");
                return View(registerDto);
            }

            //Manuel bir map leme işlemi yapıyoruz burda.
            //Form dan gelen verileri AppUser entity sindeki alanlarla eşleştiriyoruz. Az sayıda sütun olduğu için manuel yapmak daha mantıklı.
            //Auto Mapper burada gereksiz iş yükü olacağından elle yazdık.
            var user = new AppUser 
            {
                Email= registerDto.Email,
                FirstName =registerDto.FirstName,
                LastName =registerDto.LastName,
                UserName =registerDto.UserName
            };

            var result = await _userManager.CreateAsync(user,registerDto.Password);

            if (!result.Succeeded)
            {
                foreach(var error in result.Errors)
                {
                    ModelState.AddModelError(error.Code, error.Description);
                }
                return View(registerDto);
            }
            return RedirectToAction("Login");
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Bu Email sistemde kayıtlı değil");
                return View(loginDto);
            }

            var result = await _signInManager.PasswordSignInAsync(user, loginDto.Password, false, false);
            if (!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "Email veya şifre hatalı");
                return View(loginDto);
            }


            return RedirectToAction("Index", "Message");
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }


    }
}
