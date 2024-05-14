using hexasync.infrastructure.dotnetenv;
using TKSoutdoorsparts;
using TKSoutdoorsparts.Adapter;

// Add services to the container.
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddTransient<IDbFactory, DbFactory>();
builder.Services.AddTransient<IEnvReader, EnvReader>();
builder.Services.AddTransient<IOdbcDataHelper, OdbcDataHelper>();

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
