using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using MediatR;
using ReservaSalas.Application.Common.Behaviors;
using ReservaSalas.Application.Common.Interfaces;
using ReservaSalas.Application.Common.Services;

namespace ReservaSalas.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
                cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
            });
            services.AddScoped<IReservaValidator, ReservaValidator>();
            return services;
        }
    }
}
