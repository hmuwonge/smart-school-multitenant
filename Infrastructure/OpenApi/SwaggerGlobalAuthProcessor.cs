using System.Reflection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Namotion.Reflection;
using NSwag;
using NSwag.Generation.AspNetCore;
using NSwag.Generation.Processors;
using NSwag.Generation.Processors.Contexts;

namespace Infrastructure.OpenApi;

/// <summary>
/// A custom operation processor for adding global authentication support in Swagger/OpenAPI.
/// </summary>
/// <param name="schema">
/// The authentication scheme to be applied. Default to <see cref="JwtBearerDefaults.AuthenticationScheme"/>
/// </param>
public class SwaggerGlobalAuthProcessor(string schema):IOperationProcessor
{
    private readonly string _schema = schema;
    
    /// <summary>
    /// initializes a new instance of the <see cref="SwaggerGlobalAuthProcessor"/> class
    /// with the default authenitcation scheme (<see cref="JwtBearerDefaults.AuthenticationScheme"/>).
    /// </summary>
    public SwaggerGlobalAuthProcessor(): this(JwtBearerDefaults.AuthenticationScheme)
    {}
        
    public bool Process(OperationProcessorContext context)
    {
        IList<object> list = ((AspNetCoreOperationProcessorContext)context)
            .ApiDescription.ActionDescriptor.TryGetPropertyValue<IList<object>>("EndpointMetadata");

        if (list is not null)
        {
            if (list.OfType<AllowAnonymousAttribute>().Any())
            {
                return true;
            }

            if (context.OperationDescription.Operation.Security?.Count == 0 || context.OperationDescription.Operation.Security == null)
            {
                (context.OperationDescription.Operation.Security ??= [])
                    .Add(new OpenApiSecurityRequirement
                    {
                        {
                            _schema,
                            Array.Empty<string>()
                        }
                    });
            }
        }
        return true;
    }

    public static class ObjectExtensions
    {
        /// <summary>
        /// Attempts to retrieve the value of a specified property from an object.
        /// If the property does not exist or is inaccessible, returns the specified default value.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="propertyName"></param>
        /// <param name="defaultValue"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T TryGetPropertyValue<T>(object obj, string propertyName, T defaultValue = default) =>
            obj.GetType().GetRuntimeProperty(propertyName) is PropertyInfo propertyInfo
                ? (T)propertyInfo.GetValue(obj)
                : defaultValue;
    }
}