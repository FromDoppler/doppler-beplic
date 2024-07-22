namespace DopplerBeplic.Models.Responses.BeplicResponses
{
    public class BeplicPlanAssignSuccessResponse
    {
        public int Id { get; set; }
        public string StartDate { get; set; } = string.Empty;
        public string EndDate { get; set; } = string.Empty;
        public string ActiveDate { get; set; } = string.Empty;
        public bool Active { get; set; }
        public int? TrialPeriod { get; set; }
    }
}
