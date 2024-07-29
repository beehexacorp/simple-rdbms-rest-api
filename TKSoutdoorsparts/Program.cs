using hexasync.infrastructure.dotnetenv;
using System.Text.Json.Serialization;
using TKSoutdoorsparts.Constants;
using TKSoutdoorsparts.Helpers;
using TKSoutdoorsparts.Settings;

// Add services to the container.
var builder = WebApplication.CreateBuilder(args);
builder.Services
    .AddControllers()
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

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
