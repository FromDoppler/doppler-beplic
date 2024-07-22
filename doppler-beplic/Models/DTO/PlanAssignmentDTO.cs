using Newtonsoft.Json;

namespace DopplerBeplic.Models.DTO
{
    public class PlanAssignmentDTO
    {
        [JsonProperty("idExternal")]
        public required string IdExternal { get; set; }

        [JsonProperty("idPlan")]
        public required long IdPlan { get; set; }
    }
}
