using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace PropPunkShared.Core;


public abstract class ScopedServiceBase
{
    public static void AutoRegisterServices(ref ServiceCollection services,
        params Assembly[] assemblies)
    {
        ServiceBase.AutoRegisterServices(services, assemblies);
    }

    public static void AutoRegisterServices(IServiceCollection services,
        params Assembly[] assemblies)
    {
        ServiceBase.AutoRegisterServices(services, assemblies);
    }
}


public abstract class ServiceBase
{
    public static void AutoRegisterServices(ref ServiceCollection services,
        params Assembly[] assemblies)
    {
        foreach (var assembly in assemblies)
        {
            foreach (var type in assembly.GetTypes())
            {
                if (type.IsAbstract) continue;
                if (type.IsAssignableTo(typeof(ServiceBase)))
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
                if (type.IsAssignableTo(typeof(ServiceBase)))
                    services.AddSingleton(type);
                if (type.IsAssignableTo(typeof(ScopedServiceBase)))
                    services.AddScoped(type);
            }
        }
    }
}
