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
    public class OrderStatusRepository : DomainRepository<OrderStatus>, IOrderStatusRepository
    {
        public OrderStatusRepository(IDbConnection databaseConnection) : base(databaseConnection)
        {
        }

        public OrderStatusRepository(IDatabaseFactory databaseOptions) : base(databaseOptions)
        {
        }
    }
}
