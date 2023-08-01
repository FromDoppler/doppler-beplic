using Newtonsoft.Json;

namespace DopplerBeplic.Models.DTO
{
    public class UserCreationDTO
    {
        [JsonProperty("customer")]
        public required UserCreationCustomer Customer { get; set; }
        [JsonProperty("room")]
        public UserCreationRoom? Room { get; set; }
    }

    public class UserCreationCustomer
    {
        [JsonProperty("cuit")]
        public string Cuit { get; set; } = string.Empty;
        [JsonProperty("address")]
        public string Address { get; set; } = string.Empty;
        [JsonProperty("razonSocial")]
        public string BusinessName { get; set; } = string.Empty;
        [JsonProperty("legalName")]
        public string LegalName { get; set; } = string.Empty;
        [JsonProperty("idExternal")]
        public string IdExternal { get; set; } = string.Empty;
        [JsonProperty("apiKey")]
        public string ApiKey { get; set; } = string.Empty;
        [JsonProperty("plan")]
        public UserCreationPlan? Plan { get; set; }
        [JsonProperty("userAdmin")]
        public CustomerUserAdmin UserAdmin { get; set; } = new CustomerUserAdmin();
    }

    public class CustomerUserAdmin
    {
        [JsonProperty("email")]
        public string Email { get; set; } = string.Empty;
        [JsonProperty("firstName")]
        public string FirstName { get; set; } = string.Empty;
        [JsonProperty("lastName")]
        public string LastName { get; set; } = string.Empty;
        [JsonProperty("celphone")]
        public string Cellphone { get; set; } = string.Empty;
    }

    public class UserCreationRoom
    {
        [JsonProperty("name")]
        public string? RoomName { get; set; }
        [JsonProperty("group")]
        public RoomGroup? Group { get; set; }
        [JsonProperty("channel")]
        public RoomChannel? Channel { get; set; }
    }

    public class RoomGroup
    {
        [JsonProperty("name")]
        public string? GroupName { get; set; }
    }

    public class RoomChannel
    {
        [JsonProperty("name")]
        public string? ChannelName { get; set; }
    }

    public class UserCreationPlan
    {
        [JsonProperty("idPlan")]
        public int? IdPlan { get; set; }
        [JsonProperty("name")]
        public string? PlanName { get; set; }
        [JsonProperty("messageLimit")]
        public int? MessageLimit { get; set; }
    }
}
