using System;
using System.Reflection;
using hexasync.domain.managers;
using hexasync.domain.profile.models;
using hexasync.infrastructure.dotnetenv;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace SimpleRDBMSRestfulAPI
{
    public class ApplicationDbContext : DbContext
    {
        //Vault
        //End Vault
        public DbSet<ConnectionInfoModel> ConnectionInfo { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            builder.ApplyConfigurationsFromAssembly(Assembly.GetAssembly(typeof(ProfileModel))!);

            builder.Entity<ConnectionInfoModel>()
                .Property(x => x.Id)
                .HasColumnType("uuid")
                .ValueGeneratedOnAdd()
                .HasDefaultValueSql("uuid_generate_v4()")
                .IsRequired();
            builder.Entity<ConnectionInfoModel>()
                .Property(x => x.CreatedAt)
                .HasColumnType("int8")
                .ValueGeneratedOnAdd()
                .HasDefaultValueSql("FLOOR(EXTRACT(EPOCH FROM NOW()) * 1000)")
                .IsRequired();
            builder.Entity<ConnectionInfoModel>()
                .Property(x => x.UpdatedAt)
                .HasColumnType("int8")
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("FLOOR(EXTRACT(EPOCH FROM NOW()) * 1000)")
                .IsRequired();
            builder.Entity<ConnectionInfoModel>()
                .Property(x => x.DbType)
                .HasConversion<string>()
                .IsRequired();
        }
    }
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        ApplicationDbContext IDesignTimeDbContextFactory<ApplicationDbContext>.CreateDbContext(string[] args)
        {
            EnvLoader.LoadEnvs();
            var envReader = new EnvReader();
            var connectionReader = new ConnectionStringReader(envReader);

            var connectionString = connectionReader.GetConnectionString();

            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseNpgsql(connectionString);
            Console.WriteLine(connectionString);
            return new ApplicationDbContext(optionsBuilder.Options);
        }
    }
}