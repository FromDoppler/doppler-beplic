namespace DopplerBeplic.Models.Responses
{
    public class PlanResponse
    {
        public int? Id { get; set; }
        public string PlanType { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string PlanContractDate { get; set; } = string.Empty;
        public bool Publish { get; set; }
        public string StartDate { get; set; } = string.Empty;
        public string EndDate { get; set; } = string.Empty;
        public int? TrialPeriod { get; set; }
    }
}
