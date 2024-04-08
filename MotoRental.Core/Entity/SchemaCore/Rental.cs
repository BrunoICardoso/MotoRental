using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotoRental.Core.Entity.SchemaCore
{
    [Table($"{nameof(Rental)}", Schema = "core")]
    public class Rental
    {
        [Key]
        public int IdRental { get; set; }
        public int IdDeliveryPerson { get; set; }
        public int IdMoto { get; set; }
        public int IdRentalPlan { get; set; }
        public DateTime StartDate { get; set; } 
        public DateTime? EndDate { get; set; } 
        public DateTime ExpectedEndDate { get; set; } 
        public decimal DailyCost { get; set; } 
        public decimal TotalCost { get; set; }
    }

}
