using MotoRental.Core.Entity.SchemaAuth;
using MotoRental.Infrastructure.Repository.Settings.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotoRental.Infrastructure.Repository.Interface
{
    public interface IUserRoleRepository : IDomainRepository<UserRole>
    {
        Task<IEnumerable<string>> GetUserRolesAsync(int userId);
    }
}
