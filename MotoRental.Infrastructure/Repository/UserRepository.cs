using Dapper;
using MotoRental.Core.DTO.AuthService;
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
    public class UserRepository : DomainRepository<User>, IUserRepository
    {
        public UserRepository(IDbConnection databaseConnection) : base(databaseConnection)
        {
        }

        public UserRepository(IDatabaseFactory databaseOptions) : base(databaseOptions)
        {
        }


        public async Task<bool> AnyAsync()
        {
            var query = "SELECT EXISTS (SELECT 1 FROM \"auth\".\"User\")";
            var exists = await dbConn.ExecuteScalarAsync<bool>(query);
            
            return exists;
        }


        public async Task<IEnumerable<UserDTO>> GetUsersRoleAsync(int? userId = null, string username = null) 
        {
            var query = $@"SELECT u.""IdUser"",
	                              u.""Username"",
	                              u.""Active"",
	                              ur.""IdRole"" AS ""Role""
            FROM ""auth"".""User"" u
            INNER JOIN ""auth"".""UserRole"" ur  ON ur.""IdUser"" = u.""IdUser""
            {(userId != null && !string.IsNullOrWhiteSpace(username) ? $"WHERE u.\"IdUser\" = @{nameof(userId)} OR u.\"Username\" = @{nameof(username)};" : 
              userId != null ? $"WHERE u.\"IdUser\" = @{nameof(userId)};" :
             !string.IsNullOrWhiteSpace(username) ? $"WHERE u.\"Username\" = @{nameof(username)};" : string.Empty
            )}";

            var users = await dbConn.QueryAsync<UserDTO>(query, new { userId, username });
            return users;

        }
    }
}
