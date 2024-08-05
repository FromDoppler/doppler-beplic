namespace DopplerBeplic.Models.DTO
{
    public class TemplateMessageDTO
    {
        public string? PhoneNumber { get; set; }
        public string? PhoneNumberBusiness { get; set; }
        public string[]? HeaderParameters { get; set; }
        public string[]? BodyParameters { get; set; }
        public string? Link { get; set; }
    }
}
