using MotoRental.Core.DTO.AuthService;
using MotoRental.Core.Entity.SchemaAuth;
using MotoRental.Infrastructure.Repository.Settings.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotoRental.Infrastructure.Repository.Interface
{
    public interface IUserRepository : IDomainRepository<User>
    {
        Task<bool> AnyAsync();
        Task<IEnumerable<UserDTO>> GetUsersRoleAsync(int? userId = null, string username = null);
    }
}
