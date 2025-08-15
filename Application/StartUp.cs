using System.Reflection;
using Application.Pipelines;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class StartUp
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        return services
            .AddValidatorsFromAssembly(assembly)
            .AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationPipelineBehaviour<,>))
            .AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(assembly);
            });
    }
}