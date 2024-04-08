using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotoRental.Core.Entity.SchemaCore
{
    [Table($"{nameof(Order)}", Schema = "core")]
    public class Order
    {
        [Key]
        public int IdOrder { get; set; } 
        public DateTime CreationDate { get; set; }
        public decimal RaceValue { get; set; }
        public int IdOrderStatus { get; set; } 
        public int? IdDeliveryPerson { get; set; } 
    }

}
