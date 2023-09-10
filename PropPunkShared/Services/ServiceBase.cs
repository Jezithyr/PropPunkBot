using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace PropPunkShared.Services;

public abstract class ServiceBase
{
    public static void AutoRegisterServices(ref ServiceCollection services,
        params Assembly[] assemblies)
    {
        foreach (var assembly in assemblies)
        {
            foreach (var type in assembly.GetTypes())
            {
                if (!type.IsAssignableTo(typeof(ServiceBase)) || type.IsAbstract) continue;
                services.AddSingleton(type);
            }
        }
    }
}
