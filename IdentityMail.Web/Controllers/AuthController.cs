using IdentityMail.Web.DTOs.UserDtos;
using IdentityMail.Web.Entities;
using Mapster;
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
            if (!ModelState.IsValid)
            {
                return View(registerDto);
            }

            if(registerDto.Password != registerDto.ConfirmPassword)
            {
                ModelState.AddModelError(string.Empty, "Şifreler birbiri ile uyumlu değil.");
                return View(registerDto);
            }

            var user = registerDto.Adapt<AppUser>();

            var result = await _userManager.CreateAsync(user,registerDto.Password);

            if (!result.Succeeded)
            {
                foreach(var error in result.Errors)
                {
                    ModelState.AddModelError(error.Code, error.Description);
                }
                return View(registerDto);
            }
            await _userManager.AddToRoleAsync(user, "User");
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
            ViewBag.NameSurname = user.FirstName + " " + user.LastName;
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Bu Email sistemde kayıtlı değil");
                return View(loginDto);
            }

            if(user.IsActive == false)
            {
                ModelState.AddModelError(string.Empty, "Hesabınız sistem yöneticisi tarafından dondurulmuştur. Lütfen destek ile iletişime geçin.");
            }

            var result = await _signInManager.PasswordSignInAsync(user, loginDto.Password, false, false);
            if (!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "Email veya şifre hatalı");
                return View(loginDto);
            }

            var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");
            if (isAdmin)
            {
                return RedirectToAction("Dashboard", "AdminDashboard");
            }

            return RedirectToAction("Index", "Message");
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }


    }
}
