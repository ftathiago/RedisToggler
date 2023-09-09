using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Diagnostics.CodeAnalysis;

namespace RedisToggler.Api;

[ExcludeFromCodeCoverage]
public class ApplicationHeaders : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        operation.Parameters.Add(new OpenApiParameter
        {
            Name = HeaderNames.AcceptLanguage,
            In = ParameterLocation.Header,
            Description = "Inform the expected language",
            Example = new OpenApiString("pt-BR"),
            Required = false,
            Schema = new OpenApiSchema
            {
                Type = "string",
            },
        });
    }
}
