using MotoRental.Core.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotoRental.Core.DTO.RentalService
{
    public class RentalResponseDTO
    {
        public int RentalId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime ExpectedEndDate { get; set; }
        public decimal DailyCost { get; set; }
        public decimal TotalCost { get; set; }
        public RentalDeliveryPersonMotoDTO RentalDeliveryPersonMoto { get; set; }
    }
}
