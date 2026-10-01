using IdentityMail.Web.DTOs.CategoryDtos;
using IdentityMail.Web.DTOs.UserMessageDtos;
using IdentityMail.Web.Entities;

namespace IdentityMail.Web.Models
{
    public class AdminDashboardViewModel
    {
        public int TotalUser { get; set; }
        public int TodaySendMessage { get; set; }
        public int TotalMessage { get; set; }
        public int UnreadMessage { get; set; }
        public int TrashMessages { get; set; }
        public int ReportMessages { get; set; }
        public List<TopSenderDto> TopSenders { get; set; }
        public List<TopCategoryDto> TopCategories { get; set; }

    }
}
