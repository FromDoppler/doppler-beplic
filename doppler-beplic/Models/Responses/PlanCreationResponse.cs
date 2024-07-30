namespace DopplerBeplic.Models.Responses
{
    public class PlanCreationResponse
    {
        public bool Success { get; set; }
        public int? PlanId { get; set; }
        public string? Error { get; set; }
        public string? ErrorStatus { get; set; }
    }
}
