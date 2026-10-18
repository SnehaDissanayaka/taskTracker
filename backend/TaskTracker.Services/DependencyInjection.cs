using Microsoft.Extensions.DependencyInjection;
using TaskTracker.Core.Configuration;
using TaskTracker.Core.Interfaces;

namespace TaskTracker.Services;

public static class DependencyInjection
{
    public static IServiceCollection AddServicesLayer(this IServiceCollection services, TaskLimits limits)
    {
        services.AddSingleton(limits);
        services.AddScoped<ITaskService, TaskService>();
        return services;
    }
}
