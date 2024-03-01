namespace viki_01.Services;

public class MapperBuilder(IServiceCollection services)
{
    public MapperBuilder AddMapper<T>() where T : class
    {
        var mapperInterface = typeof(T).GetInterfaces().FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IMapper<,>));
        if (mapperInterface != null)
        {
            services.AddTransient(mapperInterface, typeof(T));
        }
        else
        {
            throw new InvalidOperationException($"Type {typeof(T).FullName} does not implement IMapper<,>");
        }

        return this;
    }
}