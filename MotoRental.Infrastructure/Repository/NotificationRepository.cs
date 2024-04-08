using Dapper;
using MotoRental.Core.DTO.NotificationService;
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
    public class NotificationRepository : DomainRepository<Notification>, INotificationRepository
    {
        public NotificationRepository(IDbConnection databaseConnection) : base(databaseConnection)
        {
        }

        public NotificationRepository(IDatabaseFactory databaseOptions) : base(databaseOptions)
        {
        }

        public async Task<IEnumerable<NotificationResponseDTO>> GetNotificationsAsync() 
        {
            string query = $@"SELECT 
                    n.""NotifiedAt"",
                    o.""IdOrder"", 
                    o.""CreationDate"", 
                    o.""RaceValue"", 
                    os.""IdOrderStatus"" AS ""OrderStatus"", 
                    dp.""IdDeliveryPerson"", 
                    dp.""Name"", 
                    dp.""CNPJ"", 
                    dp.""BirthDate"" AS ""DateOfBirth"", 
                    dp.""CNHNumber"", 
                    dp.""IdCNHType"" AS ""CNHType"", 
                    dp.""CNHImagePath"", 
                    dp.""Active""
                    FROM ""core"".""Notification"" n
                    JOIN ""core"".""Order"" o ON n.""IdOrder"" = o.""IdOrder""
                    JOIN ""core"".""OrderStatus"" os ON o.""IdOrderStatus"" = os.""IdOrderStatus""
                    JOIN ""core"".""DeliveryPerson"" dp ON n.""IdDeliveryPerson"" = dp.""IdDeliveryPerson""
                    ORDER BY n.""NotifiedAt"" DESC;";

            var notificationDictionary = new Dictionary<int, NotificationResponseDTO>();

            await dbConn.QueryAsync<NotificationResponseDTO, NotificationOrderDTO, NotificationDeliveryPersonDTO, NotificationResponseDTO>(
                query,
                (notification, order, deliveryPerson) =>
                {
                    NotificationResponseDTO notificationEntry;

                    if (!notificationDictionary.TryGetValue(order.IdOrder, out notificationEntry))
                    {
                        notificationEntry = notification;
                        notificationEntry.Order = order;
                        notificationEntry.DeliveryPerson = new List<NotificationDeliveryPersonDTO>();
                        notificationDictionary.Add(order.IdOrder, notificationEntry);
                    }

                    notificationEntry.DeliveryPerson.Add(deliveryPerson);
                    return notificationEntry;
                },
                splitOn: "IdOrder,IdDeliveryPerson"
            );

            return notificationDictionary.Values;
        }
    }
}
