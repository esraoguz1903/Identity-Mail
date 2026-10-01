using IdentityMail.Web.Context;
using IdentityMail.Web.DTOs.UserProfileDtos;
using IdentityMail.Web.Entities;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;


namespace IdentityMail.Web.Controllers
{
    public class ProfileController(UserManager<AppUser> _userManager,
                                   AppDbContext _context) : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> EditProfile()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var profileDto = user.Adapt<ProfileDto>();
            return View(profileDto);
        }

        [HttpPost]
        public async Task<IActionResult> EditProfile(ProfileDto profileDto)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);

            user.FirstName = profileDto.FirstName;
            user.LastName = profileDto.LastName;
            
            if (profileDto.ProfileImage != null && profileDto.ProfileImage.Length > 0)
            {
                // 1. Resmin uzantısını al (.jpg, .png vb)
                var extension = Path.GetExtension(profileDto.ProfileImage.FileName);

                // 2. Resme benzersiz bir isim ver (Aynı isimde 2 resim çakışmasın diye Guid kullanılır)
                var newImageName = Guid.NewGuid() + extension;

                // 3. Dosyanın kaydedileceği bilgisayardaki tam yolu bul
                var location = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/", newImageName);

                // 4. Resmi klasöre kopyala (Kaydet)
                using (var stream = new FileStream(location, FileMode.Create))
                {
                    await profileDto.ProfileImage.CopyToAsync(stream);
                }

                // 5. Veritabanındaki resim linkini, bu yeni oluşturduğumuz resmin adresiyle değiştir!
                user.ProfileImageUrl = "/images/" + newImageName;
            }

            var result = await _userManager.UpdateAsync(user);
            
            if (!result.Succeeded)
            {
                foreach(var item in result.Errors)
                {
                    ModelState.AddModelError(item.Code, item.Description);
                }
                return View(profileDto);
            } 
            return RedirectToAction("Index","Message");
        }
    }
}
