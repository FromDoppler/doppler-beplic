using DopplerBeplic.DopplerSecurity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

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

        services.AddAuthentication()
            .AddJwtBearer(opt =>
            {
                opt.IncludeErrorDetails = true;
                opt.Events = new JwtBearerEvents()
                {
                    //OnAuthenticationFailed = context =>
                    //{
                    //    var err = context.Exception.ToString();

                    //    Console.WriteLine(err);
                    //    return Task.CompletedTask;
                    //},
                    OnTokenValidated = ctx =>
                    {
                        Console.WriteLine();
                        Console.WriteLine("Claims from the access token");
                        if (ctx?.Principal != null)
                        {
                            foreach (var claim in ctx.Principal.Claims)
                            {
                                Console.WriteLine($"{claim.Type} - {claim.Value}");
                            }
                        }
                        Console.WriteLine();
                        return Task.CompletedTask;
                    }
                };
            });

        services.AddAuthorization();

        return services;
    }
}
