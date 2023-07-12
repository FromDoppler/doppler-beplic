namespace DopplerBeplic.Models.DTO
{
    public class UserCreationDTO
    {
        public required UserCreationCustomer Customer { get; set; }
        public UserCreationRoom? Room { get; set; }
        public UserCreationPlan? Plan { get; set; }
    }

    public class UserCreationCustomer
    {
        public string Cuit { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string BusinessName { get; set; } = string.Empty;
        public string LegalName { get; set; } = string.Empty;
        public string IdExternal { get; set; } = string.Empty;
        public string ApiKey { get; set; } = string.Empty;
        public CustomerUserAdmin UserAdmin { get; set; } = new CustomerUserAdmin();
    }

    public class CustomerUserAdmin
    {
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Cellphone { get; set; } = string.Empty;
    }

    public class UserCreationRoom
    {
        public string? RoomName { get; set; }
        public RoomGroup? Group { get; set; }
    }

    public class RoomGroup
    {
        public string? GroupName { get; set; }
    }

    public class UserCreationPlan
    {
        public int? IdPlan { get; set; }
        public string? PlanName { get; set; }
        public int? MessageLimit { get; set; }
    }
}
