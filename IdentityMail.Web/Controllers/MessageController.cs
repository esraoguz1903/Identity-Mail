using IdentityMail.Web.Context;
using IdentityMail.Web.DTOs.UserMessageDtos;
using IdentityMail.Web.Entities;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace IdentityMail.Web.Controllers
{
    [Authorize(Roles = "User")]
    public class MessageController(UserManager<AppUser> _userManager,
                                   AppDbContext _context) : Controller
    {
        public async Task<IActionResult> Index([FromQuery] MessageFilterDto filter)  //Gelen mesajları listelediğimiz sayfa
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            ViewBag.fullName = user.FirstName + " " + user.LastName;
            var query = _context.UserMessages.Include(x => x.Sender)
                                                    .Include(x => x.Category)
                                                    .Where(x => x.ReceiverId == user.Id && x.IsTrash == false)
                                                    .AsQueryable();

            query = ApplyFilters(query, filter);
            
            var messages = await ApplyPaginationAsync(query, filter);

            return View(messages);
        }


        public IActionResult SendMail()
        {
            FillCategoryList();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SendMail(SendMailDto sendMailDto, string Islem)
        {
            var sender = await _userManager.FindByNameAsync(User.Identity.Name);
            int? tempReceiverId = null;
            if (Islem == "Send")
            {
                if (string.IsNullOrWhiteSpace(sendMailDto.ReceiverMail))
                {
                    ModelState.AddModelError(string.Empty, "Göndermek için alıcı mail adresi girmek zorundasınız.");
                    FillCategoryList();
                    return View(sendMailDto);
                }
                var receiver = await _userManager.FindByEmailAsync(sendMailDto.ReceiverMail);

                if (receiver is null)
                {
                    ModelState.AddModelError(string.Empty, "Girdiğiniz mail ile sistemde kayıtlı kullanıcı bulunamadı.");
                    FillCategoryList();
                    return View(sendMailDto);
                }
                tempReceiverId = receiver.Id;
            }

            else if (Islem == "Draft")
            {
                sendMailDto.IsDraft = true;
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Geçersiz işlem.");
                FillCategoryList();
                return View(sendMailDto);
            }

            var newMessage = sendMailDto.Adapt<UserMessage>();

            //sendmaildto da olmayan ancak entity de bulunan değerleri manuel olarak eklememiz gerekiyor.
            newMessage.SendDate = DateTime.Now;
            newMessage.SenderId = sender.Id;
            newMessage.ReceiverId = tempReceiverId;

            if (string.IsNullOrWhiteSpace(newMessage.Subject))
            {
                newMessage.Subject = "(Konu Yok)";
            }
            if (string.IsNullOrWhiteSpace(newMessage.Body))
            {
                newMessage.Body = "(Boş İçerik)";
            }


            _context.UserMessages.Add(newMessage);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");


        }

        public async Task<IActionResult> DraftList()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var drafts = await _context.UserMessages.Include(x => x.Receiver).Include(x => x.Category).Where(x => x.SenderId == user.Id && x.IsDraft == true && x.IsTrash == false).ToListAsync();
            return View(drafts);
        }

        [HttpGet]
        public async Task<IActionResult> EditDraft(int id)
        {
            var draft = await _context.UserMessages.FindAsync(id);
            var draftDto = draft.Adapt<SendMailDto>();
            return View(draftDto);
        }

        [HttpPost]
        public async Task<IActionResult> EditDraft(SendMailDto sendMailDto, string Islem)
        {

            var sender = await _userManager.FindByNameAsync(User.Identity.Name);
            int? tempReceiverId = null;
            if (Islem == "Send")
            {
                if (string.IsNullOrWhiteSpace(sendMailDto.ReceiverMail))
                {
                    ModelState.AddModelError(string.Empty, "Göndermek için alıcı mail adresi girmek zorundasınız.");
                    return View(sendMailDto);
                }
                var receiver = await _userManager.FindByEmailAsync(sendMailDto.ReceiverMail);
                if (receiver == null)
                {
                    ModelState.AddModelError(string.Empty, "Girdiğiniz mail ile sistemde kayıtlı kullanıcı bulunamadı.");
                    return View(sendMailDto);
                }

                tempReceiverId = receiver.Id;
                sendMailDto.IsDraft = false;
            }

            else if (Islem == "Draft")
            {
                sendMailDto.IsDraft = true;
            }

            var draft = await _context.UserMessages.FindAsync(sendMailDto.Id);

            //Aşağıdaki şekilde usermessage nesnesini usermessage a çevirmeye çalışmışım. Bu yanlış kullanım.
            //var draftentity = draft.Adapt<UserMessage>();

            sendMailDto.Adapt(draft);
            draft.SendDate = DateTime.Now;
            draft.SenderId = sender.Id;
            draft.ReceiverId = tempReceiverId;

            if (string.IsNullOrWhiteSpace(draft.Subject))
            {
                draft.Subject = "(Konu Yok)";
            }
            if (string.IsNullOrWhiteSpace(draft.Body))
            {
                draft.Body = "(Boş İçerik)";
            }


            _context.UserMessages.Update(draft);  //Mapster ın mevcut nesnenin üstüne yazma özelliği.
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(DraftList));
        }

        public async Task<IActionResult> DeleteDraft(int id)
        {
            var draft = await _context.UserMessages.FindAsync(id);
            _context.UserMessages.Remove(draft);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(DraftList));
        }

        public async Task<IActionResult> MailDetail(int id)
        {
            var message = await _context.UserMessages.Include(x => x.Sender).Include(x => x.Category).FirstOrDefaultAsync(x => x.Id == id);

            message.IsRead = true;
            await _context.SaveChangesAsync();
            return View(message);
        }

        public async Task<IActionResult> Sentbox([FromQuery] MessageFilterDto filter)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name); //Maili gönderen kullanıcıyı bulduk burda.

            var query = _context.UserMessages.Include(x => x.Receiver).Include(x => x.Category).Where(y => y.SenderId == user.Id && y.IsDraft == false && y.IsTrash == false).AsQueryable();

            query = ApplyFilters(query, filter);

            query = query.OrderByDescending(x => x.Id);

            var messages = await ApplyPaginationAsync(query, filter); 

            return View(messages);
        }

        public async Task<IActionResult> TrashMail(int id)
        {
            var message = await _context.UserMessages.FindAsync(id);
            message.IsTrash = true;
            message.IsRead = true;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> TrashMessages()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var messages = await _context.UserMessages.Include(x => x.Sender).Include(x => x.Category).Where(x => x.ReceiverId == user.Id && x.IsTrash == true).ToListAsync();
            return View(messages);
        }

        public async Task<IActionResult> MakeImportant(int id)
        {
            var message = await _context.UserMessages.FindAsync(id);
            message.IsImpotant = !message.IsImpotant;
            await _context.SaveChangesAsync();
            return Ok();
        }

        public async Task<IActionResult> ImportantMessages()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var messages = await _context.UserMessages.Include(x => x.Sender).Include(x => x.Category).Where(x => x.ReceiverId == user.Id && x.IsImpotant == true && x.IsTrash == false).ToListAsync();
            return View(messages);
        }

        public async Task<IActionResult> RestoreMessage(int id)
        {
            var message = await _context.UserMessages.FindAsync(id);
            message.IsTrash = false;
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }


        private void FillCategoryList()
        {
            var category = _context.Categories.ToList();
            ViewBag.ctgr = (from ctgr in category
                            select new SelectListItem
                            {
                                Text = ctgr.CategoryName,
                                Value = ctgr.Id.ToString()
                            }).ToList();
        }

        private IQueryable<UserMessage> ApplyFilters(IQueryable<UserMessage> query, MessageFilterDto filter)
        {
            //Ad soyada göre arama
            if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
            {
                query = query.Where(x =>
                    (x.Sender != null && (x.Sender.FirstName.Contains(filter.SearchTerm) || x.Sender.LastName.Contains(filter.SearchTerm))) ||
                    (x.Receiver != null && (x.Receiver.FirstName.Contains(filter.SearchTerm) || x.Receiver.LastName.Contains(filter.SearchTerm)))
                );
            }

            //Konu ve içeriğe göre arama
            if (!string.IsNullOrWhiteSpace(filter.SubjectSearch))
            {
                query = query.Where(x => x.Subject.Contains(filter.SubjectSearch) ||
                                         x.Body.Contains(filter.SubjectSearch));
            }

            //Başlangıç tarihine göre arama
            if (filter.StartDate.HasValue)
            {
                query = query.Where(x => x.SendDate >= filter.StartDate.Value);
            }

            //Bitiş tarihine göre arama
            if (filter.EndDate.HasValue)
            {
                query = query.Where(x => x.SendDate <= filter.EndDate.Value);
            }

            //Okundu/Okunmadı filtresi
            if (filter.IsRead.HasValue)
            {
                query = query.Where(x => x.IsRead == filter.IsRead.Value);
            }

            //Kategoriye göre filtreleme
            if (filter.CategoryId.HasValue)
            {
                query = query.Where(x => x.CategoryId == filter.CategoryId.Value);
            }

            //Eskiden yeniye sıralama
            if (filter.SortBy == "oldest")
            {
                query = query.OrderBy(x => x.Id);
            }
            //Yeniden eskiye sıralama
            else
            {
                query = query.OrderByDescending(x => x.Id);
            }

            return query;
        }

        private async Task<List<UserMessage>> ApplyPaginationAsync(IQueryable<UserMessage> query, MessageFilterDto filter)
        {
            var totalMessages = await query.CountAsync();

            var result = await query.Skip((filter.Page - 1) * filter.PageSize)
                         .Take(filter.PageSize)
                         .ToListAsync();

            ViewBag.CurrentPage = filter.Page;
            ViewBag.TotalPages = (int)(Math.Ceiling((double)totalMessages / filter.PageSize));
            ViewBag.TotalMessages = totalMessages;
            ViewBag.PageSize = filter.PageSize;

            // Sadece o sayfada gösterilecek olan mesaj listesini geri döndür
            return result;
        }


    }
}
