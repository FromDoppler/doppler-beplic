namespace DopplerBeplic.Models.Responses
{
    public class TemplateMessageResponse
    {
        public string? MessageId { get; set; }
        public List<TemplateMessageStatusResponse>? Status { get; set; }
    }
}
