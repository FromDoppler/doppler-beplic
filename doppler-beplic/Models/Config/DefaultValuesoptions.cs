namespace DopplerBeplic.Models.Config
{
    public class DefaulValuesOptions
    {
        public const string Values = "DefaultValues";

        public DefaultRoom Room { get; set; } = new DefaultRoom();
        public DefaultPlan Plan { get; set; } = new DefaultPlan();
    }

    public class DefaultRoom
    {
        public const string Room = "Room";

        public string Name { get; set; } = string.Empty;
        public string Group { get; set; } = string.Empty;
        public string Channel { get; set; } = string.Empty;
    }

    public class DefaultPlan
    {
        public const string Plan = "Plan";

        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int MessageLimit { get; set; }
    }
}
