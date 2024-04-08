using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotoRental.Core.DTO.RentalService
{
    public class RentalDeliveryPersonMotoDTO
    {
        public RentalDeliveryPersonDTO DeliveryPerson { get; set; }
        public RentalMotoDTO Moto { get; set; }
    }
}
