namespace DopplerBeplic.Models.Config
{
    public class DefaulValuesOptions
    {
        public const string Values = "DefaultValues";

        public DefaultCustomer Customer { get; set; } = new DefaultCustomer();
        public DefaultPlan Plan { get; set; } = new DefaultPlan();
        public DefaultCustomerStatus CustomerStatus { get; set; } = new DefaultCustomerStatus();
    }

    public class DefaultCustomerStatus
    {
        public const string Customer = "CustomerStatus";

        public string Active { get; set; } = string.Empty;
        public string Inactive { get; set; } = string.Empty;
        public string Low { get; set; } = string.Empty;
    }

    public class DefaultCustomer
    {
        public const string Customer = "Customer";

        public string Partner { get; set; } = string.Empty;
        public string BusinessName { get; set; } = string.Empty;
        public string LegalName { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Cuit { get; set; } = string.Empty;
    }

    public class DefaultPlan
    {
        public const string Plan = "Plan";

        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int MessageLimit { get; set; }
    }
}
