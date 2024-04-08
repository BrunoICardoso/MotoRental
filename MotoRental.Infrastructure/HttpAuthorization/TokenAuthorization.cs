
using MotoRental.Core.DTO.AuthService;
using System.IdentityModel.Tokens.Jwt;
using System.Text.Json;

namespace MotoRental.Infrastructure.HttpAuthorization
{
    public class TokenAuthorization
    {
        public JwtTokenDTO ReadToken(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return null;
            }

            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            JwtSecurityToken jwtToken = handler.ReadJwtToken(token);

            if (!jwtToken.Claims.Any(claim => claim.Type == "IdUser"))
            {
                return null;
            }

            string payload = jwtToken.Payload.SerializeToJson();

            JwtTokenDTO tokenclient = JsonSerializer.Deserialize<JwtTokenDTO>(payload);

            return tokenclient;
        }

    }
}