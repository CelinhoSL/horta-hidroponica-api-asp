using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using Horta.Domain.Model;
using Horta_Api.Domain.Model;

namespace Horta_Api.Infrastructure.Repositories
{
    public class ConnectionContext : DbContext
    {
        public ConnectionContext() { }
        public ConnectionContext(DbContextOptions<ConnectionContext> options) : base(options) { }

        public DbSet<User> User { get; set; }
        public DbSet<UserLog> UserLog { get; set; }
        public DbSet<MainController> MainController { get; set; }
        public DbSet<TemperatureSensor> TemperatureSensor { get; set; }
        public DbSet<WaterLevelSensor> WaterLevelSensor { get; set; }
        public DbSet<LightSensor> LightSensor { get; set; }
        public DbSet<UserVerificationCode> UserVerificationCode { get; set; }
        public DbSet<Camera> Camera { get; set; }
        public DbSet<UserResetPasswordCode> UserResetPasswordCode { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                string host = Env.GetString("DB_HOST");
                string port = Env.GetString("DB_PORT");
                string database = Env.GetString("DB_NAME");
                string user = Env.GetString("DB_USER");
                string password = Env.GetString("DB_PASS");

                string connectionString = $"Host={host};Port={port};Database={database};Username={user};Password={password};";
                optionsBuilder.UseNpgsql(connectionString);
            }
        }
    }
}