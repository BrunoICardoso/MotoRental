using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotoRental.Core.DTO.RentalService
{
    public class RentalFilterDTO
    {
        public int Id { get; set; }
        
        public DateTime StarDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
