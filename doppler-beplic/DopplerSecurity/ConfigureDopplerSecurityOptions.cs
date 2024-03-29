using System.Security.Cryptography;
using System.Text.RegularExpressions;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace DopplerBeplic.DopplerSecurity;

public class ConfigureDopplerSecurityOptions : IConfigureOptions<DopplerSecurityOptions>
{
    private readonly IConfiguration _configuration;
    private readonly IFileProvider _fileProvider;

    public ConfigureDopplerSecurityOptions(IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
    {
        _configuration = configuration;
        _fileProvider = webHostEnvironment.ContentRootFileProvider;
    }

    private static string ReadToEnd(IFileInfo fileInfo)
    {
        using var stream = fileInfo.CreateReadStream();
        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }

    private static RsaSecurityKey ParseXmlString(string xmlString)
    {
        using var rsaProvider = new RSACryptoServiceProvider();
        rsaProvider.FromXmlString(xmlString);
        var rsaParameters = rsaProvider.ExportParameters(false);
        return new RsaSecurityKey(RSA.Create(rsaParameters));
    }

    public void Configure(DopplerSecurityOptions options)
    {
        var path = _configuration.GetValue(
            DopplerSecurityDefaults.PublicKeysFolderConfigKey,
            DopplerSecurityDefaults.PublicKeysFolderDefaultConfigValue);

#pragma warning disable CS8604 // Possible null reference argument.
        var filenameRegex = new Regex(_configuration.GetValue(
            DopplerSecurityDefaults.PublicKeysFilenameConfigKey,
            DopplerSecurityDefaults.PublicKeysFilenameRegexDefaultConfigValue));
#pragma warning restore CS8604 // Possible null reference argument.

#pragma warning disable CS8604 // Possible null reference argument.
        var files = _fileProvider.GetDirectoryContents(path)
            .Where(x => !x.IsDirectory && filenameRegex.IsMatch(x.Name));
#pragma warning restore CS8604 // Possible null reference argument.

        var publicKeys = files
            .Select(ReadToEnd)
            .Select(ParseXmlString)
            .ToArray();

        options.SigningKeys = publicKeys;
    }
}
