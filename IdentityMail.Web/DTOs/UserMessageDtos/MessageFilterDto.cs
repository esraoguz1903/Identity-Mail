namespace IdentityMail.Web.DTOs.UserMessageDtos
{
    public class MessageFilterDto
    {
        public string? SearchTerm { get; set; } //İsim araması için
        public string? SubjectSearch {  get; set; } // konu ve içerik araması için
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? CategoryId { get; set; }
        public bool? IsRead { get; set; }
        public string? SortBy { get; set; }  //Sıralama
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 5;
    }
}
