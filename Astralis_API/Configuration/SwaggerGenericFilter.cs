using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.ComponentModel;
using System.Reflection;

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

                if (context.ApiDescription.ActionDescriptor is ControllerActionDescriptor descriptor)
                {
                    var displayNameAttr = descriptor.ControllerTypeInfo.GetCustomAttribute<DisplayNameAttribute>();
                    if (displayNameAttr != null)
                    {
                        entityName = displayNameAttr.DisplayName;
                    }
                }

                // 1. Summary
                if (!string.IsNullOrWhiteSpace(operation.Summary))
                    operation.Summary = operation.Summary.Replace("{EntityName}", entityName);

                // 2. Description
                if (!string.IsNullOrWhiteSpace(operation.Description))
                    operation.Description = operation.Description.Replace("{EntityName}", entityName);

                // 3. Parameters (URL, Query)
                if (operation.Parameters != null)
                {
                    foreach (var param in operation.Parameters)
                    {
                        if (!string.IsNullOrWhiteSpace(param.Description))
                            param.Description = param.Description.Replace("{EntityName}", entityName);
                    }
                }

                // 4. RequestBody (Le corps du POST/PUT -> C'est celui qui te manquait !)
                if (operation.RequestBody != null && !string.IsNullOrWhiteSpace(operation.RequestBody.Description))
                {
                    operation.RequestBody.Description = operation.RequestBody.Description.Replace("{EntityName}", entityName);
                }

                // 5. Responses
                if (operation.Responses != null)
                {
                    foreach (var response in operation.Responses)
                    {
                        if (!string.IsNullOrWhiteSpace(response.Value.Description))
                            response.Value.Description = response.Value.Description.Replace("{EntityName}", entityName);
                    }
                }
            }
        }
    }
}