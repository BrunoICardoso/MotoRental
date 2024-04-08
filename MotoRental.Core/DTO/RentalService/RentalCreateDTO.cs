using MotoRental.Core.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotoRental.Core.DTO.RentalService
{
    public class RentalCreateDTO
    {
        public int DeliveryPersonId { get; set; }
        public int MotoId { get; set; }
        public DateTime StartDate { get; set; }
        public RentalPlanEnum Plan { get; set; }
        
    }
}
