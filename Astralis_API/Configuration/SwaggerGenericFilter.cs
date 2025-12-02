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

                // Calcul du pluriel
                var entityNamePlural = GetPlural(entityName);

                // Fonction locale pour faire les remplacements
                void ReplacePlaceholders(string singular, string plural)
                {
                    // 1. Summary
                    if (!string.IsNullOrWhiteSpace(operation.Summary))
                        operation.Summary = operation.Summary
                            .Replace("{EntityName}", singular)
                            .Replace("{EntityNamePlural}", plural);

                    // 2. Description
                    if (!string.IsNullOrWhiteSpace(operation.Description))
                        operation.Description = operation.Description
                            .Replace("{EntityName}", singular)
                            .Replace("{EntityNamePlural}", plural);

                    // 3. Parameters
                    if (operation.Parameters != null)
                    {
                        foreach (var param in operation.Parameters)
                        {
                            if (!string.IsNullOrWhiteSpace(param.Description))
                                param.Description = param.Description
                                    .Replace("{EntityName}", singular)
                                    .Replace("{EntityNamePlural}", plural);
                        }
                    }

                    // 4. RequestBody
                    if (operation.RequestBody != null && !string.IsNullOrWhiteSpace(operation.RequestBody.Description))
                    {
                        operation.RequestBody.Description = operation.RequestBody.Description
                            .Replace("{EntityName}", singular)
                            .Replace("{EntityNamePlural}", plural);
                    }

                    // 5. Responses
                    if (operation.Responses != null)
                    {
                        foreach (var response in operation.Responses)
                        {
                            if (!string.IsNullOrWhiteSpace(response.Value.Description))
                                response.Value.Description = response.Value.Description
                                    .Replace("{EntityName}", singular)
                                    .Replace("{EntityNamePlural}", plural);
                        }
                    }
                }

                ReplacePlaceholders(entityName, entityNamePlural);
            }
        }

        // Petite fonction utilitaire pour l'anglais
        private string GetPlural(string singular)
        {
            if (string.IsNullOrEmpty(singular)) return "";

            // Cas finissant par s, x, z, ch, sh -> on ajoute "es" (ex: Address -> Addresses)
            if (singular.EndsWith("s", StringComparison.OrdinalIgnoreCase) ||
                singular.EndsWith("x", StringComparison.OrdinalIgnoreCase) ||
                singular.EndsWith("z", StringComparison.OrdinalIgnoreCase) ||
                singular.EndsWith("ch", StringComparison.OrdinalIgnoreCase) ||
                singular.EndsWith("sh", StringComparison.OrdinalIgnoreCase))
            {
                return singular + "es";
            }

            // Cas finissant par y précédé d'une consonne -> on remplace par "ies" (ex: Category -> Categories)
            // (Simplifié ici, on assume que y à la fin est souvent à remplacer sauf "Day", "Boy" etc. mais pour des entités ça passe souvent)
            if (singular.EndsWith("y", StringComparison.OrdinalIgnoreCase) && singular.Length > 1)
            {
                // Vérif simple voyelle
                char beforeY = singular[^2];
                if (!"aeiou".Contains(char.ToLower(beforeY)))
                {
                    return singular.Substring(0, singular.Length - 1) + "ies";
                }
            }

            // Défaut -> on ajoute "s"
            return singular + "s";
        }
    }
}