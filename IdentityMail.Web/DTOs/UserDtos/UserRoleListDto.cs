namespace IdentityMail.Web.DTOs.UserDtos
{
    public class UserRoleListDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool IsActive { get; set; }

        public IList<string> Roles { get; set; }
    }
}

