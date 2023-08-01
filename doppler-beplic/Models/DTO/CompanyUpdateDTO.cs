namespace DopplerBeplic.Models.DTO
{
    public class CompanyUpdateDTO
    {
        public required CompanyUpdateData Customer { get; set; }
    }

    public class CompanyUpdateData
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

