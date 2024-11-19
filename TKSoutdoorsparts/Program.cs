using System.Text.Json.Serialization;
using hexasync.infrastructure.dotnetenv;
using Microsoft.AspNetCore.Diagnostics;
using Serilog;
using TKSoutdoorsparts.Constants;
using TKSoutdoorsparts.Helpers;
using TKSoutdoorsparts.Middleware;
using TKSoutdoorsparts.Settings;

// Add services to the container.
var builder = WebApplication.CreateBuilder(args);

// Register the log
var logPath = Path.Combine(
    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
    "HexaSyncRDBMSSimpleAPI",
    "Logs",
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

// builder.Logging.AddConsole();
// builder.Logging.AddFile("Logs/Request-{Date}.txt");

builder
    .Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

// builder.Services.AllowResolvingKeyedServicesAsDictionary();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddTransient<IEnvReader, EnvReader>();
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

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<RequestResponseLoggingMiddleware>();

app.UseExceptionHandler(a =>
    a.Run(async context =>
    {
        var logger = app.Logger;
        var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
        var exception = exceptionHandlerPathFeature?.Error;
        app.Logger.LogError(
            exception?.Message ?? exception?.InnerException?.Message,
            exception?.InnerException ?? exception
        );
    })
);

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
