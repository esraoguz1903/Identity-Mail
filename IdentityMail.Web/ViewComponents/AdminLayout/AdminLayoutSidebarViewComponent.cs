using IdentityMail.Web.Context;
using IdentityMail.Web.Entities;
using IdentityMail.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace IdentityMail.Web.ViewComponents.AdminLayout
{
    public class AdminLayoutSidebarViewComponent(AppDbContext _context,
                                                 UserManager<AppUser> _userManager) : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var user = await _userManager.FindByEmailAsync(User.Identity.Name);

            var viewModel = new AdminDashboardViewModel
            {
                ReportMessages = _context.UserMessages.Where(x => x.IsReported == true && x.IsTrash==false).Count()
            };

            return View(viewModel);
        }
    }
}
