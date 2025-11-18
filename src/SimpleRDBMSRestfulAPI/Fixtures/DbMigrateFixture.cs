using System.Threading;
using System.Threading.Tasks;
using Dapper;
using hexasync.common;
using hexasync.domain.managers;
using hexasync.libs.fixtures;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace SimpleRDBMSRestfulAPI.Fixtures
{
    [Priority(1)]
    public class DbMigrateFixture(IDbFactory dbFactory) : IFixture
    {
        private readonly IDbFactory _dbFactory = dbFactory;

        public async Task Configure(CancellationToken cancellationToken)
        {
            await using var dbContext = _dbFactory.CreateDbContext<ApplicationDbContext>(DatabaseQueryType.DML_WRITE);

            var connectionString = dbContext.Database.GetConnectionString();
            var builder = new NpgsqlConnectionStringBuilder(connectionString);
            var targetDb = builder.Database;

            // Connect to the default "postgres" database
            builder.Database = "postgres";

            await using var conn = new NpgsqlConnection(builder.ConnectionString);
            await conn.OpenAsync();

            var exists = await conn.QueryFirstOrDefaultAsync<string>(
                "SELECT datname FROM pg_database WHERE datname = @dbName",
                new { dbName = targetDb });

            if (string.IsNullOrWhiteSpace(exists))
            {
                await conn.ExecuteAsync($"CREATE DATABASE \"{targetDb}\"");
            }

            // apply extensions and migrations
            await dbContext.Database.ExecuteSqlRawAsync("CREATE EXTENSION IF NOT EXISTS \"uuid-ossp\"");
            await dbContext.Database.ExecuteSqlRawAsync("CREATE EXTENSION IF NOT EXISTS \"pg_trgm\"");
            await dbContext.Database.ExecuteSqlRawAsync("CREATE EXTENSION IF NOT EXISTS \"pgcrypto\"");
            await dbContext.Database.MigrateAsync(cancellationToken: cancellationToken);
        }
    }
}