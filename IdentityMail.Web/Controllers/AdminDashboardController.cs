using IdentityMail.Web.Context;
using IdentityMail.Web.DTOs.CategoryDtos;
using IdentityMail.Web.DTOs.UserMessageDtos;
using IdentityMail.Web.Entities;
using IdentityMail.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IdentityMail.Web.Controllers
{
    [Authorize(Roles ="Admin")]
    public class AdminDashboardController(AppDbContext _context,
                                          UserManager<AppUser> _userManager) : Controller
    {
        public IActionResult Dashboard()
        {
            var dashboard = new AdminDashboardViewModel
            {
                TotalUser = _context.Users.Count(),
                TodaySendMessage = _context.UserMessages.Where(x => x.SendDate > DateTime.Today && x.SendDate < DateTime.Today.AddDays(1)).Count(),
                TotalMessage = _context.UserMessages.Count(),
                UnreadMessage = _context.UserMessages.Where(x => x.IsRead == false).Count(),
                TrashMessages = _context.UserMessages.Where(x => x.IsTrash == true).Count(),
                

                TopCategories = _context.Categories.Select(c => new TopCategoryDto
                {
                    CategoryName = c.CategoryName,
                    MessageCount = _context.UserMessages.Where(x => x.CategoryId == c.Id).Count()
                })
                .OrderByDescending(x => x.MessageCount)
                .Take(5)
                .ToList(),

                TopSenders = _context.Users.Select(u => new TopSenderDto
                {
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    MessageCount = _context.UserMessages.Where(x => x.SenderId == u.Id).Count()

                })
                .OrderByDescending(x => x.MessageCount)
                .Take(5)
                .ToList()
            };

           
            
            return View(dashboard);
        }
    }
}
