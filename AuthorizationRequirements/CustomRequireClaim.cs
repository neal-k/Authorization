using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace AuthorizationRequirements
{
    // Creating a custom require claim for authorization
    public class CustomRequireClaim : IAuthorizationRequirement
    {

        public CustomRequireClaim(string claimType)
        {
            ClaimType = claimType;
        }

        public string ClaimType { get; private set; }
    }

    // handler that evaluates and validates the claim
    public class CustomRequireClaimHandler : AuthorizationHandler<CustomRequireClaim>
    {
        public CustomRequireClaimHandler() {}

        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context, 
            CustomRequireClaim requirement)
        {
            var hasClaim = context.User.Claims.Any(x => x.Type == requirement.ClaimType);

            if(hasClaim)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}