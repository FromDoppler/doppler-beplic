using Newtonsoft.Json;

namespace DopplerBeplic.Models.DTO
{
    public class CustomerUpdateDTO
    {
        [JsonProperty("customer")]
        public required CustomerUpdateData Customer { get; set; }
    }

    public class CustomerUpdateData
    {
        [JsonProperty("idExternal")]
        public string IdExternal { get; set; } = string.Empty;
        [JsonProperty("cuit")]
        public string? Cuit { get; set; }
        [JsonProperty("address")]
        public string? Address { get; set; }
        [JsonProperty("razonSocial")]
        public string? BusinessName { get; set; }
        [JsonProperty("legalName")]
        public string? LegalName { get; set; }
        [JsonProperty("status")]
        public string? Status { get; set; }
        [JsonProperty("isBlocked")]
        public bool? IsBlocked { get; set; }
        [JsonProperty("isCancelated")]
        public bool? IsCancelated { get; set; }
    }
}

