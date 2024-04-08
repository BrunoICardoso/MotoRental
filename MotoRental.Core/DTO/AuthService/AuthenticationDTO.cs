using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotoRental.Core.DTO.AuthService
{
    public class AuthenticationDTO
    {
        public string Token { get; set; }
        public DateTime ExpiresAt { get; set; }
        public IEnumerable<string> Roles { get; set; }
        public int UserId { get; set; }

    }
}
