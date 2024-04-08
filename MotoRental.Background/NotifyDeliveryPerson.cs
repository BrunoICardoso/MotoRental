using System;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using MotoRental.Core.DTO.NotificationService;
using MotoRental.Service.Interface;

namespace MotoRental.Background
{
    public class NotifyDeliveryPerson
    {
        private readonly ILogger<NotifyDeliveryPerson> _logger;
        private readonly INotificationService _notificationService;

        public NotifyDeliveryPerson(ILogger<NotifyDeliveryPerson> logger, INotificationService notificationService)
        {
            _logger = logger;
            _notificationService = notificationService;
        }

        
        [Function("NotifyDeliveryPerson")]
        public async Task Run([RabbitMQTrigger("notifications", ConnectionStringSetting = "ConnectionRabbitMQ")] NotificationOrderDeliveryPersonsDTO model, FunctionContext context)
        {
            try
            {
                var ret = await _notificationService.AddNotificationOrderDeliveryPersonAsync(model);
                _logger.LogInformation(ret.Message);
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
            }
        }
    }
}
