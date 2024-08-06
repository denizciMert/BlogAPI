using System.Text.Json.Serialization;
using BlogAPI.Data;
using BlogAPI.Entities.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BlogAPI
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Register the DbContext with the connection string
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString("ApplicationDbContext"),
                    sqlServerOptions => sqlServerOptions.EnableRetryOnFailure()  // Enable retry on failure for SQL Server
                ));

            // Add Identity services to the container
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            // Configure JSON options to handle reference loops and ignore null values
            builder.Services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
                options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            });

            // Configure Swagger/OpenAPI
            // Learn more at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Migrate the database
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var context = services.GetRequiredService<ApplicationDbContext>(); // Get the DbContext
                    await context.Database.MigrateAsync(); // Apply migrations
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>(); // Get the logger
                    logger.LogError(ex, "An error occurred while migrating the database."); // Log the error
                }
            }

            // Configure the HTTP request pipeline for development environment
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();  // Enable authentication
            app.UseAuthorization();  // Enable authorization

            app.MapControllers();

            await app.RunAsync();  // Run the application
        }
    }
}
