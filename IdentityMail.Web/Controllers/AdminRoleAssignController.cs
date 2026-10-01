using IdentityMail.Web.Context;
using IdentityMail.Web.DTOs.UserDtos;
using IdentityMail.Web.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Threading.Tasks;

namespace IdentityMail.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminRoleAssignController(RoleManager<AppRole> _roleManager,
                                           AppDbContext _context,
                                           UserManager<AppUser> _userManager) : Controller
    {
        public async Task<IActionResult> ListRole()
        {
            var users = await _userManager.Users.ToListAsync();
            var userList = new List<UserRoleListDto>();
            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);

                userList.Add(new UserRoleListDto
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    IsActive = user.IsActive,
                    Roles = roles
                });
            }

            var allRoles = _roleManager.Roles.ToList();
            ViewBag.Roles = (from role in allRoles
                             select new SelectListItem
                             {
                                 Text = role.Name,
                                 Value = role.Id.ToString()
                             }).ToList();

            return View(userList);
        }



        public async Task<IActionResult> AssignRole(RoleAssignDto roleAssignDto)
        {
            var user = await _userManager.FindByIdAsync(roleAssignDto.UserId.ToString());
            if (user == null)
            {
                NotFound();
            }

            var currentRole = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, currentRole);

            var role = await _roleManager.FindByIdAsync(roleAssignDto.RoleId.ToString());
            if (role != null)
            {
                await _userManager.AddToRoleAsync(user, role.Name);
            }

            return RedirectToAction(nameof(ListRole));

        }

        public async Task<IActionResult> ToggleUserStatus(int userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user != null)
            {
                user.IsActive = !user.IsActive;
                await _userManager.UpdateAsync(user);
            }
            return RedirectToAction(nameof(ListRole));
        }

        [HttpPost]
        public async Task<IActionResult> WarningUser(int senderId, int messageId)
        {
            var badUser = await _userManager.FindByIdAsync(senderId.ToString());
            if (badUser != null)
            {
                badUser.WarningCount++;

                var warningMessage = new UserMessage
                {
                    SenderId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)),
                    ReceiverId = badUser.Id,
                    Subject = "SİSTEM UYARISI: Kural İhlali",
                    Body = $"Sayın {badUser.FirstName}, göndermiş olduğunuz bir e-posta kurallarımıza aykırı bulunduğu için sistem tarafından uyarıldınız. (Mevcut Uyarı Sayınız: {badUser.WarningCount}/3)",
                    SendDate = DateTime.Now,
                    IsRead = false
                };
                


                if (badUser.WarningCount >= 3)
                {
                    badUser.IsActive = false;
                    warningMessage.Body += " - DİKKAT: 3. uyarınızı aldığınız için hesabınız kalıcı olarak dondurulmuştur.";
                }

                await _userManager.UpdateAsync(badUser);
                _context.UserMessages.Add(warningMessage);
            }
            return RedirectToAction(nameof(ListRole));
        }

    }
}
