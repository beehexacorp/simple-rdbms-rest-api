using hexasync.infrastructure.dotnetenv;
using TKSoutdoorsparts;
using TKSoutdoorsparts.Helpers;
using TKSoutdoorsparts.Settings;
using Serilog.Extensions.Logging.File;

DotNetEnv.Env.Load();
// Add services to the container.
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddTransient<IEnvReader, EnvReader>();
builder.Services.AddTransient<IOdbcDataHelper, OdbcDataHelper>();
builder.Services.AddSingleton<AppSettings, AppSettings>();


var app = builder.Build();
var loggerFactory = app.Services.GetService<ILoggerFactory>();
loggerFactory.AddFile(builder.Configuration["Logging:LogFilePath"].ToString());

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
