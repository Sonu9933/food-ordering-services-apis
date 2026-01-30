using Customer.Core.Contracts.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Customer.Core.Services
{
    public class JwtTokenService : IJwtTokenService
    {
        public JwtTokenService(IConfiguration configuration)
        {
            this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public IConfiguration configuration { get; }

        public string GenerateToken(Entity.Customer customer)
        {
            var claims = new List<Claim>
                {
                     new Claim (JwtRegisteredClaimNames.Name, customer.CustomerName),
                     new Claim (JwtRegisteredClaimNames.Email, customer.Email),
                     new Claim (JwtRegisteredClaimNames.Jti, customer.CustomerId.ToString())
                };

            var key = configuration.GetValue<string>("SecurityKey");
            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key ?? string.Empty));

            var credentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            var securityTokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = configuration.GetValue<string>("Issuer"),
                Audience = configuration.GetValue<string>("Audience"),
                Expires = DateTime.UtcNow.AddMinutes(2),
                Subject = new ClaimsIdentity(claims),
                SigningCredentials = credentials,
                IssuedAt = DateTime.Now
            };

            var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            var securityToken = jwtSecurityTokenHandler.CreateToken(securityTokenDescriptor);

            return jwtSecurityTokenHandler.WriteToken(securityToken);
        }

        public ClaimsPrincipal? ValidateToken(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(configuration.GetValue<string>("SecurityKey") ?? string.Empty);

                // Validation parameters
                var validationParameters = new TokenValidationParameters
                {
                    // Signature validation
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),

                    // Issuer validation
                    ValidateIssuer = true,
                    ValidIssuer = configuration.GetValue<string>("Issuer"),

                    // Audience validation
                    ValidateAudience = true,
                    ValidAudience = configuration.GetValue<string>("Audience"),

                    // Expiration validation
                    ValidateLifetime = true,

                    // Clock skew (allows for slight time differences between servers)
                    ClockSkew = TimeSpan.Zero // Strict: no tolerance for clock differences
                };

                // Validate and return claims
                // discard validated token as it's not needed, we only need the principal
                var principal = tokenHandler.ValidateToken(token, validationParameters, out _);
                return principal;
            }
            catch
            {
                // Token is invalid (signature mismatch, expired, etc.)
                return null;
            }
        }
    }
}
