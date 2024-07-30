namespace DopplerBeplic.Models.Responses
{
    public class PlanCancellationResponse
    {
        public bool Success { get; set; }
        public string? Error { get; set; }
        public string? ErrorStatus { get; set; }
    }
}
