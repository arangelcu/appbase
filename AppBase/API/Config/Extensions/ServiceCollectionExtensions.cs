namespace AppBase.API.Config.Extensions;

public static class ServiceCollectionExtensions
{
    /**
     * Load all service and repository classes by namespace
     */
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.Scan(scan => scan
            .FromAssemblyOf<Program>() // Usar Program como referencia
            .AddClasses(classes => classes.Where(t =>
                t.Namespace != null &&
                (t.Namespace.Contains(".Services") || t.Namespace.Contains(".services") ||
                 t.Namespace.Contains(".Repositories") || t.Namespace.Contains(".repositories"))))
            .AsImplementedInterfaces()
            .WithScopedLifetime());

        return services;
    }
}