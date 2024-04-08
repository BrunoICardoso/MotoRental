using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace MotoRental.Core.DTO.NotificationService
{
    public class NotificationOrderDeliveryPersonsDTO
    {
        public IEnumerable<int> DeliveryPersonIds { get; set; }
        public int OrderId { get; set; }
    }
}
