using Microsoft.Extensions.DependencyInjection.Extensions;

using System.Reflection;

namespace CoRSample.Api.Extensions;

public interface IEndpoint
{
    void MapEndpoint(IEndpointRouteBuilder app);
}

public static class EndpointExtensions
{
    /// <summary>
    /// Adds all endpoints in the given assembly to the service collection.
    /// </summary>
    /// <param name="services">The service collection to add the endpoints to.</param>
    /// <param name="assembly">The assembly to scan for endpoints.</param>
    /// <returns>The service collection.</returns>
    public static IServiceCollection AddEndpoints(
        this IServiceCollection services,
        Assembly assembly)
    {
        var serviceDescriptors = assembly
            .DefinedTypes
            .Where(type => type is { IsAbstract: false, IsInterface: false } &&
                           type.IsAssignableTo(typeof(IEndpoint)))
            .Select(type => ServiceDescriptor.Transient(typeof(IEndpoint), type))
            .ToArray();

        services.TryAddEnumerable(serviceDescriptors);

        return services;
    }

    /// <summary>
    /// Maps all endpoints added to the service collection to the <paramref name="app"/>.
    /// </summary>
    /// <param name="app">The web application to map the endpoints to.</param>
    /// <param name="routeGroupBuilder">The route group builder to use for mapping the endpoints. If not provided, the endpoints will be mapped directly to the <paramref name="app"/>.</param>
    /// <returns>The web application.</returns>
    public static IApplicationBuilder MapEndpoints(
        this WebApplication app,
        RouteGroupBuilder? routeGroupBuilder = null)
    {
        var endpoints = app.Services
            .GetRequiredService<IEnumerable<IEndpoint>>();

        var builder = routeGroupBuilder is null ? app : (IEndpointRouteBuilder)routeGroupBuilder;

        foreach (var endpoint in endpoints)
        {
            endpoint.MapEndpoint(builder);
        }

        return app;
    }
}