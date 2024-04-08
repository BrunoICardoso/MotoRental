using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Logging;
using MotoRental.Core.DTO.NotificationService;
using MotoRental.Core.Entity.SchemaCore;
using MotoRental.Core.Enum;
using MotoRental.Core.ResponseDefault;
using MotoRental.Infrastructure.Messaging;
using MotoRental.Infrastructure.Repository.Interface;
using MotoRental.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MotoRental.Service
{
    public class NotificationService : INotificationService
    {
        private readonly RabbitMQClient _rabbitMQClient;
        private readonly INotificationRepository _notificationRepository;
        private readonly ILogger<NotificationService> _logger;

        public NotificationService(RabbitMQClient rabbitMQClient, INotificationRepository notificationRepository, ILogger<NotificationService> logger)
        {
            _rabbitMQClient = rabbitMQClient;
            _notificationRepository = notificationRepository;
            _logger = logger;
        }

        public async Task<ReturnAPI> NotifyDeliveryPersonAsync(IEnumerable<int> deliveryPersonIds, int orderid)
        {
            var notificationMessage = JsonSerializer.Serialize(new NotificationOrderDeliveryPersonsDTO { DeliveryPersonIds = deliveryPersonIds, OrderId = orderid });
            
            _rabbitMQClient.PublishMessage(QueueNamesEnum.Notifications, notificationMessage);

            return new ReturnAPI(HttpStatusCode.OK) {Message = $"Message successfully sent to {QueueNamesEnum.Notifications}" };

        }

        public async Task<ReturnAPI> AddNotificationOrderDeliveryPersonAsync(NotificationOrderDeliveryPersonsDTO notificationOrder) 
        {
            try
            {
                foreach (int deliveryPersonId in notificationOrder.DeliveryPersonIds)
                {
                    await _notificationRepository.AddAsync(new Notification { IdDeliveryPerson = deliveryPersonId, IdOrder = notificationOrder.OrderId, NotifiedAt = DateTime.Now }) ;
                    _logger.LogInformation($" Adding Notification to DeliveryPerson: {deliveryPersonId} Order:{notificationOrder.OrderId}");
                }

                await _notificationRepository.SaveChangesAsync();

                return new ReturnAPI(HttpStatusCode.Created) {Message = $"Message processed {QueueNamesEnum.Notifications}" };
            }

            catch (Exception)
            {
                _notificationRepository.Rollback();
                throw;
            }

        }

        public async Task<ReturnAPI<IEnumerable<NotificationResponseDTO>>> GetNotificationsAsync() 
        {
            var notificationResponses = await _notificationRepository.GetNotificationsAsync();

            return new ReturnAPI<IEnumerable<NotificationResponseDTO>>(HttpStatusCode.OK,notificationResponses);
        }

        
    }
}