namespace DopplerBeplic.Models.DTO
{
    public class CustomerUpdateDTO
    {
        public required CustomerUpdateData Customer { get; set; }
    }

    public class CustomerUpdateData
    {
        public string Cuit { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string BusinessName { get; set; } = string.Empty;
        public string LegalName { get; set; } = string.Empty;
        public string IdExternal { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }
}

