using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace PropPunkShared.Core;


public abstract class ScopedServiceBase
{
    public static void AutoRegisterServices(ref ServiceCollection services,
        params Assembly[] assemblies)
    {
        SingletonServiceBase.AutoRegisterServices(services, assemblies);
    }

    public static void AutoRegisterServices(IServiceCollection services,
        params Assembly[] assemblies)
    {
        SingletonServiceBase.AutoRegisterServices(services, assemblies);
    }
}


public abstract class SingletonServiceBase
{
    public static void AutoRegisterServices(ref ServiceCollection services,
        params Assembly[] assemblies)
    {
        foreach (var assembly in assemblies)
        {
            foreach (var type in assembly.GetTypes())
            {
                if (type.IsAbstract) continue;
                if (type.IsAssignableTo(typeof(SingletonServiceBase)))
                    services.AddSingleton(type);
                if (type.IsAssignableTo(typeof(ScopedServiceBase)))
                    services.AddScoped(type);
            }
        }
    }
    public static void AutoRegisterServices(IServiceCollection services,
        params Assembly[] assemblies)
    {
        foreach (var assembly in assemblies)
        {
            foreach (var type in assembly.GetTypes())
            {
                if (type.IsAbstract) continue;
                if (type.IsAssignableTo(typeof(SingletonServiceBase)))
                    services.AddSingleton(type);
                if (type.IsAssignableTo(typeof(ScopedServiceBase)))
                    services.AddScoped(type);
            }
        }
    }
}
