using Asp.Versioning;
using ConsumerEnpoints.Services;
using Customer.Core.Services;
using FoodOrderingServices.API.CustomMiddleware;
using FoodOrderingServices.Core.Contracts.Repositories;
using FoodOrderingServices.Core.Contracts.Services;
using FoodOrderingServices.Core.Services;
using FoodOrderingServices.Infrastructure.Data;
using FoodOrderingServices.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Polly;
using System.Text;
using System.Threading.RateLimiting;

namespace FoodOrderingServices.API
{
    /// <summary>
    /// Application entry point and service configuration.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Main entry point for the application.
        /// Configures services, middleware, and starts the web host.
        /// </summary>
        /// <param name="args">Command line arguments.</param>
        /// <summary>
        /// Main entry point for the application.
        /// Configures services, middleware, and starts the web host.
        /// </summary>
        /// <param name="args">Command line arguments.</param>
        /// <summary>
        /// Main entry point for the application.
        /// Configures services, middleware, and starts the web host.
        /// </summary>
        /// <param name="args">Command line arguments.</param>
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container
            builder.Services.AddControllers();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Database Context
            // Configure Entity Framework Core with SQL Server
            builder.Services.AddDbContext<ApplicationDbContext>(
                opt => opt.UseSqlServer(builder
                .Configuration
                .GetConnectionString("ConsumerDBConnection")));

            // RateLimiter
            // Configure Rate Limiting
            // This helps to protect our API from abuse and ensures fair usage by limiting the number of requests a client can make within a specified time window
            builder.Services.AddRateLimiter(options =>
            {
                options.AddFixedWindowLimiter("limiting", opt =>
                {
                    opt.PermitLimit = 4;
                    opt.Window = TimeSpan.FromSeconds(12);
                    opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                    opt.QueueLimit = 2;
                });

                options.RejectionStatusCode = 429;
            });

            // Authentication and Authorization
            // Configure JWT Authentication
            // This allows us to secure our API endpoints and ensure that only authenticated users can access certain resources
            builder.Services.AddAuthentication(options =>
            {
                // Set the default authentication scheme to JWT Bearer
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                // Configure JWT Bearer options
                options.RequireHttpsMetadata = false; // Set to true in production
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetValue<string>("SecurityKey")?? string.Empty)),
                    ValidateIssuer = true,
                    ValidIssuer = builder.Configuration.GetValue<string>("Issuer"),
                    ValidateAudience = true,
                    ValidAudience = builder.Configuration.GetValue<string>("Audience"),
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero // No clock skew
                };
            });

            // ApiVersioning
            // Configure API Versioning
            // This allows us to manage different versions of our API and provide backward compatibility
            builder.Services.AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(1);
                options.ReportApiVersions = true;
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ApiVersionReader = ApiVersionReader.Combine(
                    new UrlSegmentApiVersionReader(),
                    new HeaderApiVersionReader("X-Api-Version"));
            })
            .AddMvc() // This is needed for controllers
            .AddApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'V";
                options.SubstituteApiVersionInUrl = true;
            });

            // Redis Caching
            // Configure Redis Caching
            // This helps to improve the performance of our API by storing frequently accessed data in a fast, in-memory cache
            builder.Services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = builder.Configuration.GetConnectionString("RedisConnection");

            });


            // Dependency Injection
            // Register application services and repositories for dependency injection
            builder.Services.AddScoped<IAuthCustomerService, AuthConsumerService>();
            builder.Services.AddScoped<IRestaurantService, RestaurantService>();
            builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
            builder.Services.AddScoped<ICustomerRepositary, CustomerRepositary>();
            builder.Services.AddScoped<IRestaurantRepositary, RestaurantRepositary>();
            builder.Services.AddScoped<IOrderService, OrderService>();
            builder.Services.AddScoped<IOrderRepositary, OrderRepositary>();


            // Resilience
            // Configure Polly Resilience Policies
            // Example: Retry with exponential backoff and circuit breaker
            builder.Services.AddResiliencePipeline("retry", builder =>
            {
                builder.AddRetry(new Polly.Retry.RetryStrategyOptions
                {
                    MaxRetryAttempts = 3,
                    BackoffType = DelayBackoffType.Exponential,
                    Delay = TimeSpan.FromSeconds(2),
                    UseJitter = true
                });

                builder.AddCircuitBreaker(new Polly.CircuitBreaker.CircuitBreakerStrategyOptions
                {
                    SamplingDuration = TimeSpan.FromSeconds(10),
                    FailureRatio = 0.9, // 90% failure rate
                    MinimumThroughput = 5, // Minimum 5 requests before evaluating
                    BreakDuration = TimeSpan.FromSeconds(5) // Break for 30 seconds
                });

                builder.AddTimeout(TimeSpan.FromSeconds(1));
            });

            var app = builder.Build();

            // Global Exception Handling via Custom Middleware
            app.UseMiddleware<ExceptionHandlingMiddleware>();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            // Enable authentication and authorization middleware
            app.UseAuthorization();

            //app.MapGet("/api/resource", () => "This endpoint is rate limited")
            //.RequireRateLimiting("fixed"); // Apply specific policy to an endpoint
            app.UseRateLimiter();

            // Apply rate limiting to all controllers
            app.MapControllers().RequireRateLimiting("limiting");

            // Start the application
            app.Run();
        }
    }
}

public partial class Program;