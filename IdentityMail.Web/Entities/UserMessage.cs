using System.ComponentModel.DataAnnotations;

namespace IdentityMail.Web.Entities
{
    public class UserMessage 
    {
        public int Id { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public DateTime SendDate { get; set; }
        public bool IsRead { get; set; }
        public bool IsImpotant { get; set; }
        public bool IsTrash {  get; set; }
        public bool IsDraft { get; set; }
        public bool IsReported { get; set; }  
        public string? ReportReason { get; set; } 
       

        public AppUser Sender { get; set; }
        public int SenderId { get; set; }
        public AppUser Receiver { get; set; }
        public int? ReceiverId { get; set; }

        public Category Category { get; set; }
        public int? CategoryId { get; set; }
    }
}
