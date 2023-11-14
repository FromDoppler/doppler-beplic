using Newtonsoft.Json;

namespace DopplerBeplic.Models.Responses
{
    public class PlanBalanceResponse
    {
        public bool Success { get; set; }

        public int? ConversationsQtyBalance { get; set; }

        public decimal? WhatsAppCreditBalance { get; set; }

        public string? Error { get; set; }

        public string? ErrorStatus { get; set; }
    }
}
