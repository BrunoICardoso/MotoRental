using Dapper;
using MotoRental.Core.Entity.SchemaAuth;
using MotoRental.Infrastructure.Repository.Interface;
using MotoRental.Infrastructure.Repository.Settings;
using MotoRental.Infrastructure.Repository.Settings.Interface;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotoRental.Infrastructure.Repository
{
    public class UserRoleRepository : DomainRepository<UserRole>, IUserRoleRepository
    {
        public UserRoleRepository(IDbConnection databaseConnection) : base(databaseConnection)
        {
        }

        public UserRoleRepository(IDatabaseFactory databaseOptions) : base(databaseOptions)
        {
        }


        public async Task<IEnumerable<string>> GetUserRolesAsync(int userId)
        {
            var query = $@"
            SELECT r.""Name"" 
            FROM ""auth"".""Role"" r
            INNER JOIN ""auth"".""UserRole"" ur ON r.""IdRole"" = ur.""IdRole""
            WHERE ur.""IdUser"" = @{nameof(userId)}";

            var roles = await dbConn.QueryAsync<string>(query, new { userId });
            return roles;
        }
    }
}