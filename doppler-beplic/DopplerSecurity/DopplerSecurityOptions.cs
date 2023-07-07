using Microsoft.IdentityModel.Tokens;

namespace DopplerBeplic.DopplerSecurity;

public class DopplerSecurityOptions
{
    public IEnumerable<SecurityKey> SigningKeys { get; set; } = System.Array.Empty<SecurityKey>();
}
