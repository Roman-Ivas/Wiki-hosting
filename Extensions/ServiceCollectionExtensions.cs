using viki_01.Services;

namespace viki_01.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMappers(this IServiceCollection services,
        Action<MapperBuilder> buildAction)
    {
        var builder = new MapperBuilder(services);
        buildAction(builder);
        return services;
    }
}