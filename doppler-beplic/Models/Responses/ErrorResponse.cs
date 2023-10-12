
namespace DopplerBeplic.Models.Responses
{
    public class ErrorResponse
    {
        public string? Status { get; set; }
        public string? Message { get; set; }
        public List<string>? Errors { get; set; }
    }
}
