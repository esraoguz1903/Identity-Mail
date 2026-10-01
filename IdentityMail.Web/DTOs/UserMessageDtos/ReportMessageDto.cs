namespace IdentityMail.Web.DTOs.UserMessageDtos
{
    public class ReportMessageDto
    {
        public int Id { get; set; }
        public string SenderFirstName { get; set; }
        public string SenderLastName { get; set; }
        public string ReceiverFirstName { get; set; }
        public string ReceiverLastName { get; set; }
        public DateTime SendDate { get; set; }
        public string Body { get; set; }
        public string ReportReason { get; set; }
        public int SenderId { get; set; } 
        public string Subject { get; set; }
    }
}
