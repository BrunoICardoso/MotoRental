using MotoRental.Core.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotoRental.Core.DTO.NotificationService
{
    public class NotificationOrderDTO
    {
        public int IdOrder { get; set; }
        public DateTime CreationDate { get; set; }
        public decimal RaceValue { get; set; }
        public OrderStatusEnum OrderStatus { get; set; }
        public NotificationDeliveryPersonDTO DeliveryPerson { get; set; }
    }
}
