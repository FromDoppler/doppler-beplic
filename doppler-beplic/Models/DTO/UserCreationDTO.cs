
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
        public string Cuit { get; set; }
        public string Address { get; set; }
        public string BusinessName { get; set; }
        public string LegalName { get; set; }
        public string IdExternal { get; set; }
        public string ApiKey { get; set; }
        public CustomerUserAdmin UserAdmin { get; set; }
    }

    public class CustomerUserAdmin
    {
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Celphone { get; set; }
    }

    public class UserCreationRoom
    {
        public string RoomName { get; set; }
        public RoomGroup Group { get; set; }
    }

    public class RoomGroup
    {
        public string GroupName { get; set; }
    }

    public class UserCreationPlan
    {
        public int IdPlan { get; set; }
        public string PlanName { get; set; }
        public int MessageLimit { get; set; }
    }
}
