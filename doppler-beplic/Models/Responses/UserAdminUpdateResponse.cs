namespace DopplerBeplic.Models.Responses
{
    public class UserAdminUpdateResponse
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public int? CustomerId { get; set; }
        public int? UserId { get; set; }
        public string? Error { get; set; }
        public string? ErrorStatus { get; set; }
    }
}
