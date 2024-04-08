using Dapper;
using MotoRental.Core.DTO.OrderService;
using MotoRental.Core.Entity.SchemaCore;
using MotoRental.Infrastructure.Repository.Interface;
using MotoRental.Infrastructure.Repository.Settings;
using MotoRental.Infrastructure.Repository.Settings.Interface;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotoRental.Infrastructure.Repository
{
    public class OrderRepository : DomainRepository<Order>, IOrderRepository
    {
        public OrderRepository(IDbConnection databaseConnection) : base(databaseConnection)
        {
        }

        public OrderRepository(IDatabaseFactory databaseOptions) : base(databaseOptions)
        {
        }

        public async Task<OrderResponseDTO> GetOrderDetailsAsync(int orderId)
        {
            var query = $@"SELECT
           o.""IdOrder"" AS ""Id"",
           o.""CreationDate"" AS ""CreationDate"",
           o.""RaceValue"" AS ""RaceValue"",
           os.""Status"" AS ""OrderStatus"",
           dp.""IdDeliveryPerson"",
           dp.""Name"",
           dp.""CNPJ"",
           dp.""BirthDate"",
           dp.""CNHNumber"",
           dp.""IdCNHType"" AS ""CNHType"",
           dp.""CNHImagePath"",
           dp.""Active""
         FROM core.""Order"" o
         LEFT JOIN core.""OrderStatus"" os ON o.""IdOrderStatus"" = os.""IdOrderStatus""
         LEFT JOIN core.""DeliveryPerson"" dp ON o.""IdDeliveryPerson"" = dp.""IdDeliveryPerson""
         WHERE o.""IdOrder"" = @orderId";

            var orderDetails = await dbConn.QueryAsync<OrderResponseDTO, OrderDeliveryPersonDTO, OrderResponseDTO>(
                               query,
                               (order, deliveryPerson) =>
                               {
                                   order.DeliveryPerson = deliveryPerson;
                                   return order;
                               },
                               splitOn: "IdDeliveryPerson",
                               param: new { orderId });

            return orderDetails.FirstOrDefault();
        }

    }
}
