using MotoRental.Core.Entity.SchemaCore;
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
    public class MotoRepository : DomainRepository<Moto>, IMotoRepository
    {
        public MotoRepository(IDbConnection databaseConnection) : base(databaseConnection)
        {
        }

        public MotoRepository(IDatabaseFactory databaseOptions) : base(databaseOptions)
        {
        }
    }
}
