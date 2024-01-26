namespace DopplerBeplic.Models.Config
{
    public class ApiConnectionOptions
    {
        public const string Connection = "ApiConnection";

        public string User { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string BaseUrl { get; set; } = string.Empty;
        public string PartnerKey { get; set; } = string.Empty;
    }
}
