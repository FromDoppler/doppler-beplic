namespace DopplerBeplic.Models.Config
{
    public class DefaulValuesOptions
    {
        public const string Values = "DefaultValues";

        public DefaultRoom Room { get; set; }
        public DefaultPlan Plan { get; set; }
    }

    public class DefaultRoom
    {
        public const string Room = "Room";

        public string Name { get; set; } = string.Empty;
        public string Group { get; set; } = string.Empty;
    }

    public class DefaultPlan
    {
        public const string Plan = "Plan";

        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int MessageLimit { get; set; }
    }
}
