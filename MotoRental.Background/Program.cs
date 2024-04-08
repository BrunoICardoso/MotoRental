using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using MotoRental.Core.Appsettings;
using MotoRental.Infrastructure.Messaging;
using MotoRental.Infrastructure.Repository;
using MotoRental.Infrastructure.Repository.Interface;
using MotoRental.Infrastructure.Repository.Settings;
using MotoRental.Infrastructure.Repository.Settings.Interface;
using MotoRental.Service;
using MotoRental.Service.Interface;
using RabbitMQ.Client;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureAppConfiguration((context, builder) =>
        {
            builder.SetBasePath(context.HostingEnvironment.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();
        })

    .ConfigureServices((context, services) =>
    {
        IConfiguration configuration = context.Configuration;
        
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();
        services.AddOptions();
        services.AddLogging();

        #region Configuração Context DB
        services.Configure<DataSettings>(configuration.GetSection("ConnectionStrings"));
        services.AddScoped<IDatabaseFactory, DatabaseFactory>();
        #endregion

        #region Configuration RabbitMQ

        services.Configure<RabbitMQOptions>(configuration.GetSection("RabbitMQ"));
        services.AddSingleton<RabbitMQClient>(sp =>
        {
            var rabbitMQOptions = sp.GetRequiredService<IOptions<RabbitMQOptions>>().Value;
            return new RabbitMQClient(rabbitMQOptions.HostName, rabbitMQOptions.UserName, rabbitMQOptions.Password);
        });
        
        services.AddRabbitMQQueue(configuration);
        #endregion

        #region Services Internal

        services.AddScoped<IDeliveryPersonService, DeliveryPersonService>();
        services.AddScoped<IMotoService, MotoService>();
        services.AddScoped<INotificationService, NotificationService>();
        services.AddScoped<IOrderService, OrderService>();
        services.AddScoped<IRentalService, RentalService>();
        services.AddScoped<IAuthService, AuthService>();

        #endregion

        #region Repositories

        #region Schema Core
        services.AddTransient<ICNHTypeRepository, CNHTypeRepository>();
        services.AddTransient<IDeliveryPersonRepository, DeliveryPersonRepository>();
        services.AddTransient<IMotoRepository, MotoRepository>();
        services.AddTransient<IOrderRepository, OrderRepository>();
        services.AddTransient<IOrderStatusRepository, OrderStatusRepository>();
        services.AddTransient<IRentalRepository, RentalRepository>();
        services.AddTransient<IRentalPlanRepository, RentalPlanRepository>();
        services.AddTransient<INotificationRepository, NotificationRepository>();
        #endregion

        #region Schema Auth
        services.AddTransient<IUserRepository, UserRepository>();
        services.AddTransient<IUserRoleRepository, UserRoleRepository>();
        services.AddTransient<IRoleRepository, RoleRepository>();
        #endregion

        #endregion

    })
    .Build();

host.Run();


public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRabbitMQQueue(this IServiceCollection services, IConfiguration configuration)
    {
        var rabbitMQOptions = new RabbitMQOptions();
        configuration.GetSection("RabbitMQ").Bind(rabbitMQOptions);

        var factory = new ConnectionFactory()
        {
            HostName = rabbitMQOptions.HostName,
            UserName = rabbitMQOptions.UserName,
            Password = rabbitMQOptions.Password
        };

        using (var connection = factory.CreateConnection())
        using (var channel = connection.CreateModel())
        {
            channel.QueueDeclare(queue: "notifications",
                                 durable: true,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);
        }

        return services;
    }
}