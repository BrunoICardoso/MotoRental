using Dapper;
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
    public class DeliveryPersonRepository : DomainRepository<DeliveryPerson>, IDeliveryPersonRepository
    {
        public DeliveryPersonRepository(IDbConnection databaseConnection) : base(databaseConnection)
        {
        }

        public DeliveryPersonRepository(IDatabaseFactory databaseOptions) : base(databaseOptions)
        {
        }

        public async Task<IEnumerable<int>> GetEligibleDeliveryPersonsForOrder()
        {
            string query = $@"SELECT dp.""IdDeliveryPerson""
                            FROM ""core"".""DeliveryPerson"" dp
                            INNER JOIN ""core"".""Rental"" r ON dp.""IdDeliveryPerson"" = r.""IdDeliveryPerson""
                            LEFT JOIN ""core"".""Order"" o ON dp.""IdDeliveryPerson"" = o.""IdDeliveryPerson"" AND o.""IdOrderStatus"" IN (SELECT ""IdOrderStatus"" FROM ""core"".""OrderStatus"" WHERE ""IdOrderStatus"" = 2)
                            INNER JOIN ""core"".""CNHType"" cht ON dp.""IdCNHType"" = cht.""IdCNHType""
                            WHERE dp.""Active"" = true
                            AND r.""EndDate"" IS NULL 
                            AND o.""IdOrder"" IS NULL";

            return await dbConn.QueryAsync<int>(query);
        }
    }
}
