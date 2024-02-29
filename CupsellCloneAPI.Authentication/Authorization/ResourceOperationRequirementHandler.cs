using System.Security.Claims;
using CupsellCloneAPI.Database.Entities.Product;
using Microsoft.AspNetCore.Authorization;

namespace CupsellCloneAPI.Authentication.Authorization
{
    public class ResourceOperationRequirementHandler : AuthorizationHandler<ResourceOperationRequirement, Offer>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            ResourceOperationRequirement requirement,
            Offer offer
        )
        {
            if (requirement.ResourceOperation is ResourceOperation.READ)
            {
                context.Succeed(requirement);
            }

            var userId = context.User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (userId is not null)
            {
                if (offer.Seller.Id == Guid.Parse(userId))
                {
                    context.Succeed(requirement);
                }
            }

            return Task.CompletedTask;
        }
    }
}