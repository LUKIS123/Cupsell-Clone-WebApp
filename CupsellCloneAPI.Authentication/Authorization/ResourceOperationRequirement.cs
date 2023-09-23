using Microsoft.AspNetCore.Authorization;

namespace CupsellCloneAPI.Authentication.Authorization
{
    public enum ResourceOperation
    {
        CREATE = 1,
        READ = 2,
        UPDATE = 3,
        DELETE = 4
    }

    public class ResourceOperationRequirement : IAuthorizationRequirement
    {
        public ResourceOperationRequirement(ResourceOperation resourceOperation)
        {
            ResourceOperation = resourceOperation;
        }

        public ResourceOperation ResourceOperation { get; init; }
    }
}