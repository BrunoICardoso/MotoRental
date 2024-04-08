using MotoRental.Core.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotoRental.Core.DTO.OrderService
{
    public class OrderResponseDTO
    {
        public int Id { get; set; }
        public DateTime CreationDate { get; set; }
        public decimal RaceValue { get; set; }
        public OrderStatusEnum OrderStatus { get; set; }
        public OrderDeliveryPersonDTO DeliveryPerson { get; set; }
    }



}
