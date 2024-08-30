using Newtonsoft.Json;

namespace DopplerBeplic.Models.DTO
{
    public class PlanCreationDTO
    {
        [JsonProperty("planType")]
        public required string PlanType { get; set; }

        [JsonProperty("name")]
        public required string Name { get; set; }

        [JsonProperty("status")]
        public required string Status { get; set; }

        [JsonProperty("price")]
        public required decimal Price { get; set; }

        [JsonProperty("planContractDate")]
        public required string PlanContractDate { get; set; }

        [JsonProperty("startDate")]
        public required string StartDate { get; set; }

        [JsonProperty("endDate")]
        public required string EndDate { get; set; }

        [JsonProperty("isFree")]
        public required string IsFree { get; set; }

        [JsonProperty("trialPeriod")]
        public required int TrialPeriod { get; set; }

        [JsonProperty("planConfigurations")]
        public required List<PlanConfigurationDTO> PlanConfigurations { get; set; }

        [JsonProperty("publish")]
        public bool Publish { get; set; } = false;
    }

    public class PlanConfigurationDTO
    {
        [JsonProperty("id")]
        public required int Id { get; set; }

        [JsonProperty("name")]
        public required string Name { get; set; }

        [JsonProperty("value")]
        public required string Value { get; set; }
    }
}
