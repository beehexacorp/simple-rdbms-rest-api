using hexasync.infrastructure.dotnetenv;
using TKSoutdoorsparts.Factory;
using TKSoutdoorsparts.Helpers;
using TKSoutdoorsparts.Settings;

// Add services to the container.
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
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

builder.Services.AddSingleton<BaseDataHelper, SqlAnywhereDataHelper>();
builder.Services.AddSingleton<BaseDataHelper, SqlServerDataHelper>();
builder.Services.AddSingleton<BaseDataHelper, OracleDataHelper>();
builder.Services.AddSingleton<BaseDataHelper, MySqlDataHelper>();
builder.Services.AddSingleton<BaseDataHelper, PostgresDataHelper>();

builder.Services.AddSingleton<IConnectionFactory, ConnectionFactory>();
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
