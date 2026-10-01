using Microsoft.AspNetCore.Mvc;

namespace IdentityMail.Web.ViewComponents.Filter
{
    public class FilterViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
