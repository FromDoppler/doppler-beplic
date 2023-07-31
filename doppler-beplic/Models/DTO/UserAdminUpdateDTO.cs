namespace DopplerBeplic.Models.DTO
{
    public class UserAdminUpdateDTO
    {
        public required UserAdminData UserAdmin { get; set; }
    }

    public class UserAdminData
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string CellPhone { get; set; } = string.Empty;
    }
}

