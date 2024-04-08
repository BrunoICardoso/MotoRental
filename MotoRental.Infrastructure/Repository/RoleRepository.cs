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
    public class RoleRepository : DomainRepository<Role>, IRoleRepository
    {
        public RoleRepository(IDbConnection databaseConnection) : base(databaseConnection)
        {
        }

        public RoleRepository(IDatabaseFactory databaseOptions) : base(databaseOptions)
        {
        }
    }
}
