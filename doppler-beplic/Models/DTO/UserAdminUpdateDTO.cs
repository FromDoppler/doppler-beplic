using Newtonsoft.Json;

namespace DopplerBeplic.Models.DTO
{
    public class UserAdminUpdateDTO
    {
        [JsonProperty("idExternal")]
        public string IdExternal { get; set; } = string.Empty;
        [JsonProperty("userAdmin")]
        public required UserAdminData UserAdmin { get; set; }
    }

    public class UserAdminData
    {
        [JsonProperty("email")]
        public string Email { get; set; } = string.Empty;
        [JsonProperty("firstName")]
        public string FirstName { get; set; } = string.Empty;
        [JsonProperty("lastName")]
        public string LastName { get; set; } = string.Empty;
        [JsonProperty("celphone")]
        public string CellPhone { get; set; } = string.Empty;
    }
}

