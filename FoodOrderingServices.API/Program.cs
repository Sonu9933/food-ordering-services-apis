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
using StackExchange.Redis;
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
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container
            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                    options.JsonSerializerOptions.ReferenceHandler =
                        System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles);

            // CORS: allow the React dev server to call the API
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("ReactApp", policy =>
                {
                    policy.WithOrigins("http://localhost:3000")
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                });
            });

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
            // Configure Redis Caching with ICacheRepository
            var redisConnectionString = builder.Configuration.GetConnectionString("RedisConnection") ?? "localhost:6379";
            var options_redis = ConfigurationOptions.Parse(redisConnectionString);
            options_redis.AbortOnConnectFail = false;
            
            var connectionMultiplexer = ConnectionMultiplexer.Connect(options_redis);
            builder.Services.AddSingleton<IConnectionMultiplexer>(connectionMultiplexer);
            builder.Services.AddScoped<ICacheRepository, RedisRepository>();

            // Dependency Injection
            // Register application services and repositories for dependency injection
            builder.Services.AddScoped<IAuthCustomerService, AuthConsumerService>();
            builder.Services.AddScoped<IRestaurantService, RestaurantService>();
            builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
            builder.Services.AddScoped<ICustomerRepositary, CustomerRepositary>();
            builder.Services.AddScoped<IRestaurantRepositary, RestaurantRepositary>();
            builder.Services.AddScoped<IOrderService, OrderService>();
            builder.Services.AddScoped<IOrderRepositary, OrderRepositary>();
            builder.Services.AddScoped<IPaymentService, PaymentService>();
            builder.Services.AddScoped<IPaymentRepositary, PaymentRepositary>();    

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

            // Enable CORS for React dev server
            app.UseCors("ReactApp");

            // Enable authentication and authorization middleware
            app.UseAuthorization();

            //app.MapGet("/api/resource", () => "This endpoint is rate limited")
            //.RequireRateLimiting("fixed"); // Apply specific policy to an endpoint

            app.MapControllers();

            await app.RunAsync();
        }
    }
}

public partial class Program;