using Microsoft.AspNetCore.Mvc;

namespace IdentityMail.Web.ViewComponents.AdminLayout
{
    public class AdminLayoutHeaderViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
