using Microsoft.Extensions.Options;
using MotoRental.API.ConfigController;
using MotoRental.API.Middleware;
using MotoRental.Core.Appsettings;
using MotoRental.Infrastructure.Messaging;
using Serilog;
using Serilog.Events;
using System.Text.Json.Serialization;
using FluentValidation;
using FluentValidation.AspNetCore;
using MotoRental.Infrastructure.Repository.Settings.Interface;
using MotoRental.Infrastructure.Repository.Settings;
using MotoRental.Service.Interface;
using MotoRental.Service;
using MotoRental.Infrastructure.Repository;
using MotoRental.Infrastructure.Repository.Interface;
using MotoRental.Core.Exceptions;
using Microsoft.AspNetCore.Hosting;
using FluentMigrator.Runner;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using MotoRental.Core.Cost;
using Npgsql;
using MotoRental.Infrastructure.Migrations;
using Microsoft.Extensions.DependencyInjection;
using FluentMigrator.Infrastructure;
using System;
using Microsoft.OpenApi.Models;
using System.Reflection;
using Newtonsoft.Json.Linq;
using MotoRental.Infrastructure.HttpAuthorization;
using MotoRental.Core.DTO.AuthService;
using Microsoft.AspNetCore.Mvc;

#region builder

var builder = WebApplication.CreateBuilder(args);

#region Serilog

Log.Logger = new LoggerConfiguration()
.MinimumLevel.Debug()
.MinimumLevel.Override("Microsoft", LogEventLevel.Information)
.Enrich.FromLogContext()
.WriteTo.Console()
.CreateLogger();


builder.Host.UseSerilog()
            .ConfigureAppConfiguration((hostingContext, config) =>
            {
                config.AddEnvironmentVariables();
            });
#endregion

#region Create Folder Local

var fileStorageSection = builder.Configuration.GetSection("FileStorageSettings");
var filestorageSettings = fileStorageSection.Get<FileStorageSettings>();
builder.Services.Configure<FileStorageSettings>(fileStorageSection);
Directory.CreateDirectory(filestorageSettings.BasePath);

#endregion

#region Configuration Auth

var jwtSection = builder.Configuration.GetSection("JwtSettings");
var jwtSettings = jwtSection.Get<JwtSettings>();
builder.Services.Configure<JwtSettings>(jwtSection);

builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization(options =>
    {
        options.AddPolicy(RoleConst.AdminOnly, policy => policy.RequireRole("Admin"));
        options.AddPolicy(RoleConst.EntregadorOnly, policy => policy.RequireRole("Entregador"));
        options.AddPolicy(RoleConst.AdminOrEntregador, policy => policy.RequireAssertion(context => context.User.IsInRole("Admin") || context.User.IsInRole("Entregador")));
    });

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddTransient(provider =>
{
    var token = provider.GetService<IHttpContextAccessor>().HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
    var jwtTokenReader = new TokenAuthorization();
    var jwtTokenDTO = jwtTokenReader.ReadToken(token);

    if (jwtTokenDTO is null)
    {
        return new JwtTokenDTO();
    }

    return jwtTokenDTO;
});

#endregion

#region Configuration of Routes, Controllers and MVC

builder.Services.AddMvc()
 .AddJsonOptions(options =>
 {
     options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
 });

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddRouting(options => options.LowercaseUrls = true);

#endregion

#region Configuration RabbitMQ
builder.Services.Configure<RabbitMQOptions>(builder.Configuration.GetSection("RabbitMQ"));

builder.Services.AddSingleton<RabbitMQClient>(sp =>
{
    var rabbitMQOptions = sp.GetRequiredService<IOptions<RabbitMQOptions>>().Value;
    return new RabbitMQClient(rabbitMQOptions.HostName, rabbitMQOptions.UserName, rabbitMQOptions.Password);
});

#endregion

#region Configuração Context DB
DatabaseManager.EnsureDatabaseCreated(builder.Configuration);
builder.Services.AddOptions();
builder.Services.Configure<DataSettings>(builder.Configuration.GetSection("ConnectionStrings"));
builder.Services.AddScoped<IDatabaseFactory, DatabaseFactory>();

builder.Services
    .AddFluentMigratorCore()
    .ConfigureRunner(rb => rb
        .AddPostgres()
        .WithGlobalConnectionString(builder.Configuration.GetSection("ConnectionStrings:ContextBase").Value)
        .ScanIn(typeof(CreateInitialTables).Assembly).For.Migrations())
    .AddLogging(lb => lb.AddFluentMigratorConsole()).BuildServiceProvider(false);

#endregion

#region FluentValidation

builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.Configure<ApiBehaviorOptions>(opt =>
{
    opt.SuppressModelStateInvalidFilter = true;
});


#endregion

#region Services Internal

builder.Services.AddScoped<IDeliveryPersonService, DeliveryPersonService>();
builder.Services.AddScoped<IMotoService, MotoService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IRentalService, RentalService>();
builder.Services.AddScoped<IAuthService, AuthService>();

#endregion

#region Repositories

#region Schema Core
builder.Services.AddTransient<ICNHTypeRepository, CNHTypeRepository>();
builder.Services.AddTransient<IDeliveryPersonRepository, DeliveryPersonRepository>();
builder.Services.AddTransient<IMotoRepository, MotoRepository>();
builder.Services.AddTransient<IOrderRepository, OrderRepository>();
builder.Services.AddTransient<IOrderStatusRepository, OrderStatusRepository>();
builder.Services.AddTransient<IRentalRepository, RentalRepository>();
builder.Services.AddTransient<IRentalPlanRepository, RentalPlanRepository>();
builder.Services.AddTransient<INotificationRepository, NotificationRepository>();
#endregion

#region Schema Auth
builder.Services.AddTransient<IUserRepository, UserRepository>();
builder.Services.AddTransient<IUserRoleRepository, UserRoleRepository>();
builder.Services.AddTransient<IRoleRepository, RoleRepository>();
#endregion

#endregion

#region Swagger
builder.Services.AddSwaggerGen(s =>
{
    s.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "API Moto Rental ",
        Description = "",
        Contact = new OpenApiContact
        {
            Name = "Moto",
            Email = "MOto@gmail.com",

        },
        License = new OpenApiLicense
        {
            Name = "MIT",
        }

    });

    s.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme (Example: 'Bearer 12345abcdef')",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    s.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });

    //s.SchemaFilter<AddFluentValidationRules>();

    s.OperationFilter<SwaggerDefaultValues>();

    // integrate xml comments
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    s.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));

    // allows same class names in different namespaces
    s.CustomSchemaIds(x => x.FullName);

    s.SchemaFilter<FluentValidationSwaggerSchemaFilter>();

});
#endregion

#endregion builder

#region app

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseMiddleware<ExceptionMiddleware>();

app.UseRouting();
app.UseAuthorization();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
    runner.MigrateUp();
}

app.Run();

#endregion

public static class DatabaseManager
{
    public static void EnsureDatabaseCreated(IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("ContextBase");
        var builder = new NpgsqlConnectionStringBuilder(connectionString);
        var databaseName = builder.Database;
        builder.Database = "postgres"; // Conectar ao banco padrão para verificar a existência do banco de dados alvo

        using (var connection = new NpgsqlConnection(builder.ConnectionString))
        {
            connection.Open();
            var exists = false;
            using (var command = new NpgsqlCommand($"SELECT 1 FROM pg_database WHERE datname='{databaseName}'", connection))
            {
                using (var reader = command.ExecuteReader())
                {
                    exists = reader.HasRows;
                }
            }

            if (!exists)
            {
                using (var command = new NpgsqlCommand($"CREATE DATABASE \"{databaseName}\"", connection))
                {
                    command.ExecuteNonQuery();
                }

                Console.WriteLine($"Banco de dados '{databaseName}' criado com sucesso.");
            }
        }
    }
}