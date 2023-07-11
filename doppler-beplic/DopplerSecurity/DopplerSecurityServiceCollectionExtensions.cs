using DopplerBeplic.DopplerSecurity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Microsoft.Extensions.DependencyInjection;

public static class DopplerSecurityServiceCollectionExtensions
{
    public static IServiceCollection AddDopplerSecurity(this IServiceCollection services)
    {
        services.AddSingleton<IAuthorizationHandler, IsSuperUserAuthorizationHandler>();
        services.AddSingleton<IAuthorizationHandler, IsOwnResourceAuthorizationHandler>();

        services.ConfigureOptions<ConfigureDopplerSecurityOptions>();

        services.AddAuthorizationBuilder()
            .AddDefaultPolicy(Policies.Default, policy =>
                policy
                    .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                    .RequireAuthenticatedUser());

        services.AddAuthorizationBuilder()
            .AddPolicy(Policies.OnlySuperuser, policy =>
                policy
                    .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                    .AddRequirements(new DopplerAuthorizationRequirement()
                    {
                        AllowSuperUser = true
                    })
                    .RequireAuthenticatedUser());

        services.AddAuthorizationBuilder()
            .AddPolicy(Policies.OwnResourceOrSuperuser, policy =>
                policy
                    .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                    .AddRequirements(new DopplerAuthorizationRequirement()
                    {
                        AllowSuperUser = true,
                        AllowOwnResource = true
                    })
                    .RequireAuthenticatedUser());

        services.AddAuthentication()
            .AddJwtBearer();

        services.AddOptions<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme)
            .Configure<IOptions<DopplerSecurityOptions>>((o, securityOptions) =>
            {
                o.SaveToken = true;
                o.TokenValidationParameters = new TokenValidationParameters()
                {
                    IssuerSigningKeys = securityOptions.Value.SigningKeys,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                };
            });

        services.AddAuthorization();

        return services;
    }
}
