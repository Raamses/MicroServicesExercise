using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using System.Text;

public partial class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddOptions();

        // Swagger configuration
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "Users Service", Version = "v1" });

            // Define the security scheme for JWT Bearer authentication
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "JWT Authorization header using the Bearer scheme",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey
            });

            // Add the security requirement to require JWT Bearer authentication
            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    new string[] { }
                }
            });
        });

        // Database context configuration
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
        builder.Services.AddDbContext<UserContext>(options =>
            options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

        builder.Services.AddHttpClient();


        // JWT service configuration
        string secretKey = "1qaz2wsx3ed4rf6tygh7u7654@#$SDFSDeasd4";
        builder.Services.AddSingleton<IJWTService>(new JWTService(builder.Services.BuildServiceProvider().GetRequiredService<ILogger<JWTService>>(), builder.Configuration, new HttpContextAccessor()));

        // Authentication and authorization
        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = "Bearer";
            options.DefaultChallengeScheme = "Bearer";
        }).AddJwtBearer("Bearer", options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secretKey))
            };
            options.RequireHttpsMetadata = false; // If using in development
        });
        builder.Services.AddAuthorization();

        // Endpoint discovery and API versioning
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddControllers();

        var app = builder.Build();

        // Middleware order adjustments
        app.UseStaticFiles(); // Serve static files (including Swagger UI assets)
        app.UseRouting();    // Enable routing for request matching
        app.UseAuthentication();
        app.UseAuthorization(); // Enforce authorization policies
        app.Use(async (context, next) =>
        {
            await next();
            var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();

            // Log authentication events
            if (context.User.Identity.IsAuthenticated)
            {
                logger.LogInformation("Authenticated user: {NameIdentifier}", context.User.Identity.Name);
            }
            else
            {
                logger.LogInformation("User authentication failed");
            }
        });
        app.UseSwagger();     // Enable Swagger middleware
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "Users Service");
        });   // Serve Swagger UI

        app.MapControllers();

        app.Run();
    }
}