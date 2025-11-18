using System.Text.Json;
using System.Text.Json.Serialization;
using AutoMapper;
using MessagePack.AspNetCoreMvcFormatter;
using MessagePack.Resolvers;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.FileProviders;
using Serilog;
using SimpleRDBMSRestfulAPI;
using SimpleRDBMSRestfulAPI.Helpers;
using SimpleRDBMSRestfulAPI.Libs;
using SimpleRDBMSRestfulAPI.Middleware;
using SimpleRDBMSRestfulAPI.Settings;
using hexasync.infrastructure.dotnetenv;
using hexasync.domain.managers;
using System.Data;
using MyDbType = SimpleRDBMSRestfulAPI.Constants.DbType;
using Microsoft.EntityFrameworkCore;
using hexasync.libs.fixtures;
using SimpleRDBMSRestfulAPI.Fixtures;

DotNetEnv.Env.Load();

// Add services to the container.
var builder = WebApplication.CreateBuilder(args);

// Register the log
var logPath = Path.Join(Directory.GetCurrentDirectory(), "log.txt");

// Log by day
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.File(logPath, rollingInterval: RollingInterval.Hour)
    .CreateLogger();

// Configure logging
builder.Logging.ClearProviders();
builder.Logging.AddSerilog();

builder.Services.AddSignalR(); // Add SignalR services

// Get dynamic port from arguments, environment variables, or use a default value
var port = args.Length > 0 ? args[0] :
           Environment.GetEnvironmentVariable("PORT") ??
           builder.Configuration["Kestrel:Port"] ?? "5000";
// Configure Kestrel to use the dynamic port
builder.WebHost.ConfigureKestrel(options =>
{
    Log.Logger.Information(@$"Listening to http(s)://localhost:{port}");
    options.ListenLocalhost(int.Parse(port)); // Bind to the specified port
});

builder.WebHost.UseIISIntegration();

builder
    .Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    })
    .AddMvcOptions(option =>
    {
        option.OutputFormatters.Add(new MessagePackOutputFormatter(ContractlessStandardResolver.Options));
        option.InputFormatters.Add(new MessagePackInputFormatter(ContractlessStandardResolver.Options));
    });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IAppSettings, AppSettings>();

builder.Services.AddSingleton<SqlAnywhereDataHelper>();
builder.Services.AddSingleton<SqlServerDataHelper>();
builder.Services.AddSingleton<OracleDataHelper>();
builder.Services.AddSingleton<MySqlDataHelper>();
builder.Services.AddSingleton<PostgresDataHelper>();
builder.Services.AddSingleton<ISqlInjectionHelper, SqlInjectionHelper>();

builder.Services.AddKeyedTransient<IDataHelper, SqlAnywhereDataHelper>(MyDbType.SQL_ANYWHERE);
builder.Services.AddKeyedTransient<IDataHelper, SqlServerDataHelper>(MyDbType.SQL_SERVER);
builder.Services.AddKeyedTransient<IDataHelper, OracleDataHelper>(MyDbType.ORACLE);
builder.Services.AddKeyedTransient<IDataHelper, MySqlDataHelper>(MyDbType.MYSQL);
builder.Services.AddKeyedTransient<IDataHelper, PostgresDataHelper>(MyDbType.POSTGRES);

if (OperatingSystem.IsWindows())
{
    builder.Host.UseWindowsService();
}

builder.Services.AddCors(options =>
{
    if (builder.Environment.IsDevelopment())
    {
        var frontendPort = Environment.GetEnvironmentVariable("FRONTEND_PORT") ?? "3000";
        Log.Logger.Information($"Accepted Frontend Port {frontendPort}");
        options.AddPolicy("AllowFrontend", policy =>
        {
            policy.WithOrigins($"https://localhost:{frontendPort}", $"http://localhost:{frontendPort}")
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        });
    }
});

builder.Services.AddSingleton(provider =>
{
    var autoMapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.AllowNullCollections = true;
            cfg.AllowNullDestinationValues = true;
            // cfg.AddMaps(typeof(ConnectionInfoMapper).Assembly);
            cfg.AddProfile(new ConnectionInfoMapper(provider));
        });

    autoMapperConfig.AssertConfigurationIsValid();
    var mapper = autoMapperConfig.CreateMapper();
    return mapper;
});

builder.Services.AddSingleton<IEnvReader, EnvReader>();
builder.Services.AddSingleton<IConnectionStringReader, ConnectionStringReader>();
builder.Services.AddSingleton<DbFactory>();
builder.Services.AddSingleton<IDbFactory, DbFactory>();

builder.Services.AddDbContext<ApplicationDbContext>((p, o) =>
                {
                    var connectionStringReader = p.GetService<IConnectionStringReader>();
                    if (connectionStringReader == null)
                    {
                        throw new NoNullAllowedException($"Service ConnectionStringReader is not registered");
                    }
                    var loggerFactory = p.GetService<ILoggerFactory>();
                    o.UseLoggerFactory(loggerFactory);
                    o.EnableSensitiveDataLogging();
                    o.EnableDetailedErrors();
                    o.UseNpgsql(connectionStringReader.GetConnectionString(true), b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName));
                }, ServiceLifetime.Transient);
builder.Services.AddTransient<IFixture, DbMigrateFixture>();

var app = builder.Build();

// Resolve AppSettings from DI
var appSettings = app.Services.GetRequiredService<IAppSettings>();
MachineAESEncryptorExtensions.appSettings = appSettings;


// Log by day
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.File("log.txt", shared: true, flushToDiskInterval: TimeSpan.FromSeconds(1), rollingInterval: RollingInterval.Hour)
    .WriteTo.SignalR(app.Services.GetRequiredService<IHubContext<NotificationHub>>())
    .CreateLogger();

// Serve static files under wwwroot/dashboard/dist
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Join(builder.Environment.WebRootPath, "dashboard", "dist")),
    RequestPath = "/dashboard"
});

// Explicitly map /dashboard/favicon.ico to the correct file
app.Map("/dashboard/favicon.ico", appBuilder =>
{
    appBuilder.Run(async context =>
    {
        var filePath = Path.Join(builder.Environment.WebRootPath, "dashboard", "dist", "favicon.ico");
        if (System.IO.File.Exists(filePath))
        {
            context.Response.ContentType = "image/x-icon";
            await context.Response.SendFileAsync(filePath);
        }
        else
        {
            context.Response.StatusCode = StatusCodes.Status404NotFound;
        }
    });
});

// Fallback to index.html for SPA routes
var hubBuilder = app.MapHub<NotificationHub>("/notification-hub"); // Endpoint for SignalR hub
if (builder.Environment.IsDevelopment())
{
    hubBuilder.RequireCors("AllowFrontend");
}
app.MapFallbackToFile("/dashboard/{*path:nonfile}", "dashboard/dist/index.html");

app.UseCors("AllowFrontend");

// Configure the HTTP request pipeline.
// if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<RequestResponseLoggingMiddleware>();

app.UseExceptionHandler(a => a.Run(async context =>
    {
        var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
        var exception = exceptionHandlerPathFeature?.Error;
        app.Logger.LogError(exception, exception?.Message);
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = StatusCodes.Status400BadRequest;
        await context.Response.WriteAsync(JsonSerializer.Serialize(new
        {
            errorMessage = exception?.InnerException?.Message ?? exception?.Message
        }));
    }));

var applicationLifetime = app.Services.GetRequiredService<IHostApplicationLifetime>();
applicationLifetime.ApplicationStarted.Register(() =>
{
    // Output the port and URLs the application is running on
    var serverAddressesFeature = app.Services.GetService<Microsoft.AspNetCore.Hosting.Server.Features.IServerAddressesFeature>();
    if (serverAddressesFeature != null)
    {
        foreach (var address in serverAddressesFeature.Addresses)
        {
            app.Logger.LogInformation("Application is running on: {address}", address);
        }
    }
    // Optional: Log environment
    app.Logger.LogInformation("Environment: {EnvironmentName}", builder.Environment.EnvironmentName);
    app.Logger.LogInformation("Application started successfully.");
});

applicationLifetime.ApplicationStopped.Register(() =>
{
    // Optional: Log environment
    app.Logger.LogInformation("Environment: {EnvironmentName}", builder.Environment.EnvironmentName);
    app.Logger.LogInformation("Application stopped successfully.");
});


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// ------------------------------------------------------
// ✔ RUN FIXTURES USING REAL SCOPE — no BuildServiceProvider()
// ------------------------------------------------------

using (var scope = app.Services.CreateScope())
{
    var provider = scope.ServiceProvider;

    var fixtures = provider.GetServices<IFixture>()
        .OrderBy(f =>
        {
            var attr = f.GetType()
                .GetCustomAttributes(typeof(PriorityAttribute), true)
                .FirstOrDefault() as PriorityAttribute;
            return attr?.Priority ?? 10000;
        });

    foreach (var fixture in fixtures)
    {
        await fixture.Configure(CancellationToken.None);
    }
}

app.Run();

