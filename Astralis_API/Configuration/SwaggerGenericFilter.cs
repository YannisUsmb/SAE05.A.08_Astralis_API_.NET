using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Astralis_API.Configuration
{
    public class SwaggerGenericFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var type = context.MethodInfo.DeclaringType;
            if (type != null && type.IsGenericType)
            {
                var entityType = type.GetGenericArguments()[0];
                var entityName = entityType.Name;

                if (!string.IsNullOrWhiteSpace(operation.Summary))
                {
                    operation.Summary = operation.Summary.Replace("{EntityName}", entityName);
                }
            }
        }
    }
}
