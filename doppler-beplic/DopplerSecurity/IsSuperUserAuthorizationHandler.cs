using Microsoft.AspNetCore.Authorization;

namespace DopplerBeplic.DopplerSecurity;

public partial class IsSuperUserAuthorizationHandler : AuthorizationHandler<DopplerAuthorizationRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, DopplerAuthorizationRequirement requirement)
    {
        if (requirement.AllowSuperUser && IsSuperUser(context))
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }

    private static bool IsSuperUser(AuthorizationHandlerContext context)
    {
        if (!context.User.HasClaim(c => c.Type.Equals(DopplerSecurityDefaults.SuperuserJwtKey, StringComparison.Ordinal)))
        {
            return false;
        }

#pragma warning disable CS8602 // Dereference of a possibly null reference.
        var isSuperUser = bool.Parse(context.User.FindFirst(c => c.Type.Equals(DopplerSecurityDefaults.SuperuserJwtKey, StringComparison.Ordinal)).Value);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
        return isSuperUser;
    }
}
