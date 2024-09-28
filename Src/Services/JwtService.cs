using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using RestApi.Src.Dto;
using RestApi.Src.Config;

namespace RestApi.Src.Services
{
    public class JwtService
    {
        private readonly IConfiguration _config;
        private readonly string SecretToken;
        private readonly Secret secret;

        public JwtService(IConfiguration config)
        {
            _config = config;
            SecretToken = _config["JwtConfig:SecretToken"] ?? string.Empty;
            secret = new (config);
        }

        private static ClaimsIdentity GenerateClaim(JwtClaimDto userClaim)
        {
            var claims = new ClaimsIdentity();
            claims.AddClaim(new Claim(ClaimTypes.NameIdentifier, userClaim.UserId.ToString()));
            claims.AddClaim(new Claim(ClaimTypes.Email, userClaim.Email));

            return claims;
        }

        public string GenerateAccessToken(JwtClaimDto claim, DateTime expirationTime)
        {
            var key = Encoding.ASCII.GetBytes(secret.GetJwtSecretToken());
            var credential = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature
            );
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = GenerateClaim(claim),
                Expires = expirationTime,
                SigningCredentials = credential,
            };

            var handler = new JwtSecurityTokenHandler();
            var token = handler.CreateToken(tokenDescriptor);
            return handler.WriteToken(token);
        }
    }
}
