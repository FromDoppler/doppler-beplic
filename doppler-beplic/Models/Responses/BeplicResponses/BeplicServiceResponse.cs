namespace DopplerBeplic.Models.Responses.BeplicResponses
{
    public class BeplicServiceResponse<T>
    {
        public int HttpStatusCode { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
    }
}
