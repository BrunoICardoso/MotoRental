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
    public class RentalPlanRepository : DomainRepository<RentalPlan>, IRentalPlanRepository
    {
        public RentalPlanRepository(IDbConnection databaseConnection) : base(databaseConnection)
        {
        }

        public RentalPlanRepository(IDatabaseFactory databaseOptions) : base(databaseOptions)
        {
        }
    }
}
