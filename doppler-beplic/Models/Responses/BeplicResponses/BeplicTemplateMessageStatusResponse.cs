namespace DopplerBeplic.Models.Responses.BeplicResponses
{
    public class BeplicTemplateMessageStatusResponse
    {
        public string? Status { get; set; }
        public DateTime StatusDate { get; set; }
        public BeplicTemplateMessageStatusDetailResponse? Detail { get; set; }
    }
}
