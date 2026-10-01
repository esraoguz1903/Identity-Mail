using IdentityMail.Web.Context;
using IdentityMail.Web.DTOs.UserMessageDtos;
using IdentityMail.Web.Entities;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace IdentityMail.Web.Controllers
{
    //[Authorize(Roles = "Admin")]
    public class AdminReportController(AppDbContext _context,
                                       UserManager<AppUser> _userManager) : Controller
    {
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var reportMessages = _context.UserMessages.Include(x => x.Sender)
                                                       .Include(x => x.Receiver)
                                                       .Where(x => x.IsReported == true && x.IsTrash == false)
                                                       .OrderByDescending(x => x.Id)
                                                       .ToList();
            var reportMessagesDto = reportMessages.Adapt<List<ReportMessageDto>>();
            return View(reportMessagesDto);
        }

        [HttpPost]
        public async Task<IActionResult> ReportComplaint(int id, string Reason)
        {
            var reportMessage = await _context.UserMessages.FindAsync(id);
            if (reportMessage != null)
            {
                reportMessage.IsReported = true;
                reportMessage.ReportReason = Reason;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index", "Message");
        }

        [HttpGet]
        public async Task<IActionResult> InspectMessage(int id)
        {
            var reportMessage = await _context.UserMessages.Include(x => x.Sender)
                                                           .Include(x => x.Receiver)
                                                           .FirstOrDefaultAsync(x => x.Id == id);   
            if (reportMessage == null)
            {
                NotFound();
            }
            
            var message = reportMessage.Adapt<ReportMessageDto>();
            return View(message);

        }

        public async Task<IActionResult> DismissReport(int id)
        {
            var reportMessages = await _context.UserMessages.FindAsync(id);
            if (reportMessages != null)
            {
                reportMessages.IsReported = false;
                reportMessages.ReportReason = null;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index", "AdminReport");
        }


    }
}
