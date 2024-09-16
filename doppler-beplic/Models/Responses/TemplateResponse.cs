namespace DopplerBeplic.Models.Responses
{
    public class TemplateResponse
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Language { get; set; }
        public string? Category { get; set; }
        public string? BodyText { get; set; }
        public string? HeaderType { get; set; }
        public string? HeaderText { get; set; }
        public int HeaderAmount { get; set; }
        public int BodyAmount { get; set; }
        public string? FooterText { get; set; }
        public string? PublicPreviewUrl { get; set; }
        public string? ParameterHeader { get; set; }
    }
}
