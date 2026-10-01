using IdentityMail.Web.Entities;

namespace IdentityMail.Web.DTOs.CategoryDtos
{
    public class ListCategoryDto
    {
        public int Id { get; set; }
        public string CategoryIcon { get; set; }
        public string CategoryName { get; set; }
        public string Color { get; set; }


        public ICollection<UserMessage> UserMessages { get; set; }
    }
}
