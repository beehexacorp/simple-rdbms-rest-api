using System.Text.Json;
using System.Text.Json.Serialization;
using MessagePack.AspNetCoreMvcFormatter;
using MessagePack.Resolvers;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.FileProviders;
using Serilog;
using SimpleRDBMSRestfulAPI;
using SimpleRDBMSRestfulAPI.Constants;
using SimpleRDBMSRestfulAPI.Helpers;
using SimpleRDBMSRestfulAPI.Middleware;
using SimpleRDBMSRestfulAPI.Settings;

DotNetEnv.Env.Load();

// Add services to the container.
var builder = WebApplication.CreateBuilder(args);

// Register the log
var logPath = Path.Combine(
    Directory.GetCurrentDirectory(),
    "log.txt"
);

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

builder.Services.AddKeyedTransient<IDataHelper, SqlAnywhereDataHelper>(DbType.SQL_ANYWHERE);
builder.Services.AddKeyedTransient<IDataHelper, SqlServerDataHelper>(DbType.SQL_SERVER);
builder.Services.AddKeyedTransient<IDataHelper, OracleDataHelper>(DbType.ORACLE);
builder.Services.AddKeyedTransient<IDataHelper, MySqlDataHelper>(DbType.MYSQL);
builder.Services.AddKeyedTransient<IDataHelper, PostgresDataHelper>(DbType.POSTGRES);

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

var app = builder.Build();

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
        Path.Combine(builder.Environment.WebRootPath, "dashboard", "dist")),
    RequestPath = "/dashboard"
});

// Explicitly map /dashboard/favicon.ico to the correct file
app.Map("/dashboard/favicon.ico", appBuilder =>
{
    appBuilder.Run(async context =>
    {
        var filePath = Path.Combine(builder.Environment.WebRootPath, "dashboard", "dist", "favicon.ico");
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
if (app.Environment.IsDevelopment())
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

app.Run();
