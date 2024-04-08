using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MotoRental.Core.DTO.AuthService
{
    public class JwtTokenDTO
    {
        [JsonPropertyName("unique_name")]
        public string UniqueName { get; set; }

        [JsonPropertyName("IdUser")]
        public int IdUser { get; set; }

        [JsonPropertyName("role")]
        public string Role { get; set; }

        [JsonPropertyName("nbf")]
        public int Nbf { get; set; }

        [JsonPropertyName("exp")]
        public int Exp { get; set; }

        [JsonPropertyName("iat")]
        public int Iat { get; set; }

        [JsonPropertyName("iss")]
        public string Iss { get; set; }

        [JsonPropertyName("aud")]
        public string Aud { get; set; }
    }

}
