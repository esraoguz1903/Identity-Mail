using IdentityMail.Web.Context;
using IdentityMail.Web.Entities;
using IdentityMail.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace IdentityMail.Web.ViewComponents.Layout
{
    public class LayoutSidebarViewComponent(AppDbContext _context,
                                            UserManager<AppUser> _userManager) : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var viewModel = new SidebarViewModel
            {
                UnreadSendMessage = _context.UserMessages.Where(x => x.ReceiverId == user.Id).Count(x => x.IsRead == false),

                FirstName= user.FirstName,
                LastName= user.LastName,
                ProfileImageUrl= user.ProfileImageUrl
            };
            return View(viewModel);
        }
    }
}
