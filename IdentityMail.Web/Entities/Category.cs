namespace IdentityMail.Web.Entities
{
    public class Category
    {
        public int Id { get; set; }
        public string CategoryIcon { get; set; }
        public string CategoryName { get; set; }
        public string Color { get; set; }
        

        public ICollection<UserMessage> UserMessages { get; set; }
    }
}
