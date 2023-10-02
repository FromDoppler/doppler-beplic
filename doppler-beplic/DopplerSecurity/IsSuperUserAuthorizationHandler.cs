using Microsoft.AspNetCore.Authorization;

namespace DopplerBeplic.DopplerSecurity;

public partial class IsSuperUserAuthorizationHandler : AuthorizationHandler<DopplerAuthorizationRequirement>
{
    private readonly ILogger<IsSuperUserAuthorizationHandler> _logger;

    [LoggerMessage(0, LogLevel.Debug, "The token hasn't super user permissions.")]
    partial void LogDebugTokenHasNotSuperuserPermissions();

    [LoggerMessage(1, LogLevel.Debug, "The token super user permissions is false.")]
    partial void LogDebugTokenSuperuserPermissionsIsFalse();

    public IsSuperUserAuthorizationHandler(ILogger<IsSuperUserAuthorizationHandler> logger)
    {
        _logger = logger;
    }

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, DopplerAuthorizationRequirement requirement)
    {
        if (requirement.AllowSuperUser && IsSuperUser(context))
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }

    private bool IsSuperUser(AuthorizationHandlerContext context)
    {
        if (!context.User.HasClaim(c => c.Type.Equals(DopplerSecurityDefaults.SuperuserJwtKey, StringComparison.Ordinal)))
        {
            LogDebugTokenHasNotSuperuserPermissions();
            return false;
        }

#pragma warning disable CS8602 // Dereference of a possibly null reference.
        var isSuperUser = bool.Parse(context.User.FindFirst(c => c.Type.Equals(DopplerSecurityDefaults.SuperuserJwtKey, StringComparison.Ordinal)).Value);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
        if (isSuperUser)
        {
            return true;
        }

        LogDebugTokenSuperuserPermissionsIsFalse();
        return false;
    }
}
