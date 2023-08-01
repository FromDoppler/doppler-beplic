namespace DopplerBeplic.Models.Responses
{
    public class UserCreationResponse
    {
        public bool Success { get; set; }
        public int? CustomerId { get; set; }
        public string? Error { get; set; }
        public string? ErrorStatus { get; set; }
        public string? UserToken { get; set; }
    }
}
