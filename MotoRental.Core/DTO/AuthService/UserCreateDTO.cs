using MotoRental.Core.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotoRental.Core.DTO.AuthService
{
    public class UserCreateDTO
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public RoleEnum Role { get; set; }
    }
}
