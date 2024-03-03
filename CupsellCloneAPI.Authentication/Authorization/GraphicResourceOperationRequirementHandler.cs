using System.Security.Claims;
using CupsellCloneAPI.Database.Entities.Product;
using Microsoft.AspNetCore.Authorization;

namespace CupsellCloneAPI.Authentication.Authorization
{
    public class
        GraphicResourceOperationRequirementHandler : AuthorizationHandler<ResourceOperationRequirement, Graphic>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
            ResourceOperationRequirement requirement,
            Graphic graphic)
        {
            if (requirement.ResourceOperation is ResourceOperation.READ)
            {
                context.Succeed(requirement);
            }

            var userId = context.User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (userId is not null)
            {
                if (graphic.Seller.Id == Guid.Parse(userId))
                {
                    context.Succeed(requirement);
                }
            }

            return Task.CompletedTask;
        }
    }
}