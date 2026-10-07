using Microsoft.Extensions.DependencyInjection;
using TaskTracker.Core.Interfaces;

namespace TaskTracker.Services;

public static class DependencyInjection
{
    public static IServiceCollection AddServicesLayer(this IServiceCollection services)
    {
        services.AddScoped<ITaskService, TaskService>();
        return services;
    }
}
