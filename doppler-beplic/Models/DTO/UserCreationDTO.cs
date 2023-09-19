using Newtonsoft.Json;

namespace DopplerBeplic.Models.DTO
{
    public class UserCreationDTO
    {
        [JsonProperty("customer")]
        public required UserCreationCustomer Customer { get; set; }
    }

    public class UserCreationCustomer
    {
        [JsonProperty("partner")]
        public string? Partner { get; set; }
        [JsonProperty("cuit")]
        public string? Cuit { get; set; }
        [JsonProperty("address")]
        public string? Address { get; set; }
        [JsonProperty("razonSocial")]
        public string? BusinessName { get; set; }
        [JsonProperty("legalName")]
        public string? LegalName { get; set; }
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
