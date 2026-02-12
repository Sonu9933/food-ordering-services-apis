using Asp.Versioning;
using ConsumerEnpoints.Services;
using Customer.Core.Contracts.Repositories;
using Customer.Core.Contracts.Services;
using Customer.Core.Services;
using Customer.Infrastructure.Data;
using Customer.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Threading.RateLimiting;

namespace Customer.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddDbContext<CustomerDbContext>(
                opt => opt.UseSqlServer(builder
                .Configuration
                .GetConnectionString("ConsumerDBConnection")));

            // RateLimiter
            // Configure Rate Limiter Window
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
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
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
            // Configure Api Versioning
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

            //Redis cache
            builder.Services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = builder.Configuration.GetConnectionString("RedisConnection");

            });

            builder.Services.AddScoped<IAuthCustomerService, AuthConsumerService>();
            builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
            builder.Services.AddScoped<ICustomerRepositary, CustomerRepositary>();

            var app = builder.Build();

            app.UseMiddleware<ExceptionHandlingMiddleware>();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            //app.MapGet("/api/resource", () => "This endpoint is rate limited")
            //.RequireRateLimiting("fixed"); // Apply specific policy to an endpoint
            app.UseRateLimiter();

            app.MapControllers().RequireRateLimiting("limiting");

            app.Run();
        }
    }
}

public partial class Program;