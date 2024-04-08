using MotoRental.Core.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotoRental.Core.DTO.NotificationService
{
    public class NotificationResponseDTO
    {
        public DateTime NotifiedAt { get; set; }
        public NotificationOrderDTO Order { get; set; }
        public List<NotificationDeliveryPersonDTO> DeliveryPerson { get; set; }
    }




}
