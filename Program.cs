using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using RaceDay.Data;
using RaceDay.DbConnection;
using System.Text;

namespace RaceDay
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);


            // Add services to the container.
            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            // Swagger
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.AddSecurityDefinition("Bearer",
                    new OpenApiSecurityScheme
                    {
                        Name = "Authorization",
                        Type = SecuritySchemeType.Http,
                        Scheme = "bearer",
                        BearerFormat = "JWT",
                        In = ParameterLocation.Header,
                        Description = "Enter your JWT token."
                    });

                options.AddSecurityRequirement(document =>
                    new OpenApiSecurityRequirement
                    {
                        [new OpenApiSecuritySchemeReference("Bearer", document)] = []
                    });
            });

            // JWT authentication
            string? jwtKey = builder.Configuration["Jwt:Key"];

            if (string.IsNullOrWhiteSpace(jwtKey) || jwtKey.Length < 32)
            {
                throw new InvalidOperationException(
                    "Configure Jwt:Key with a secret of at least 32 characters.");
            }

            builder.Services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters =
                        new TokenValidationParameters
                        {
                            ValidateIssuer = true,
                            ValidIssuer = "RaceDay",

                            ValidateAudience = true,
                            ValidAudience = "RaceDayUsers",

                            ValidateIssuerSigningKey = true,
                            IssuerSigningKey = new SymmetricSecurityKey(
                                Encoding.UTF8.GetBytes(jwtKey)),

                            ValidateLifetime = true,
                            ClockSkew = TimeSpan.Zero
                        };
                });

            builder.Services.AddAuthorization();

            var connectionString = builder.Configuration.GetConnectionString("RaceDayConnection")
            ?? throw new InvalidOperationException("RaceDay connection string was not found.");

            builder.Services.AddDbContext<RaceDayContext>(options => options.UseSqlServer(connectionString));

            builder.Services.AddScoped<Db_connection>();

            var app = builder.Build();


            app.MapGet("/db-check-users", async (RaceDayContext db) =>
            {
                var users = await db.Users
                    .AsNoTracking()
                    .Select(u => new
                    {
                        u.UserId,
                        u.FirstName,
                        u.LastName,
                        u.EmailAddress,
                        u.username,
                        u.Role
                    })
                    .Take(5)
                    .ToListAsync();

                return Results.Ok(users);
            });

            app.MapGet("/db-check-events", async (RaceDayContext db) =>
            {
                var events = await db.Events
                    .AsNoTracking()
                    .Select(e => new //e:event
                    {
                        e.EventId,
                        e.EventName,
                        e.Description,
                        e.EventDate,
                        e.Location,
                        e.EventType,
                        e.Organiser,
                    })
                    .Take(5)
                    .ToListAsync();

                return Results.Ok(events);
            });

            app.MapGet("/db-check", async (RaceDayContext db) =>
            {
                bool connected = await db.Database.CanConnectAsync();

                return connected
                    ? Results.Ok("EF Core connected to RaceDay.")
                    : Results.Problem("Could not connect to RaceDay.");
            });

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
