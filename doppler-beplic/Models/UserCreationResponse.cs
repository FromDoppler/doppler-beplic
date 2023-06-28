// unset:error

namespace DopplerBeplic.Models
{
    public class UserCreationResponse
    {
        public bool Success { get; set; }
        public int? CustomerId { get; set; }
        public string? Error { get; set; }
        public string? ErrorStatus { get; set; }
    }
}
