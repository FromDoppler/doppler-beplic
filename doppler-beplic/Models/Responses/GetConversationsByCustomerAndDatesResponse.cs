namespace DopplerBeplic.Models.Responses
{
    public class GetConversationsByCustomerAndDatesResponse
    {
        public bool Success { get; set; }
        public int? Conversations { get; set; }
        public string? Error { get; set; }
        public string? ErrorStatus { get; set; }
    }
}
