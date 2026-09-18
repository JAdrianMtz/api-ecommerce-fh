using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ApiEcommerce.Swagger
{
    public class AuthorizationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (!context.ApiDescription.ActionDescriptor
                    .EndpointMetadata.OfType<AuthorizeAttribute>().Any())
            {
                return;
            }

            if (context.ApiDescription.ActionDescriptor
                    .EndpointMetadata.OfType<AllowAnonymousAttribute>().Any())
            {
                return;
            }

            var scheme = new OpenApiSecuritySchemeReference(
                "Bearer",
                context.Document
            );

            operation.Security =
            [
                new OpenApiSecurityRequirement
                {
                    [scheme] = []
                }
            ];
        }
    }
}
