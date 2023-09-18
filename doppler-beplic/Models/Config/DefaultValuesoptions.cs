namespace DopplerBeplic.Models.Config
{
    public class DefaulValuesOptions
    {
        public const string Values = "DefaultValues";

        public DefaultCustomer Customer { get; set; } = new DefaultCustomer();
        public DefaultPlan Plan { get; set; } = new DefaultPlan();
    }

    public class DefaultCustomer
    {
        public const string Customer = "Customer";

        public string Partner { get; set; } = "DOPPLER";

        public string Address { get; set; } = "Sin definir";
    }

    public class DefaultPlan
    {
        public const string Plan = "Plan";

        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int MessageLimit { get; set; }
    }
}
