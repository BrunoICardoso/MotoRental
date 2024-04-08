using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotoRental.Core.Entity.SchemaCore
{
    [Table($"{nameof(OrderStatus)}", Schema = "core")]
    public class OrderStatus
    {
        [Key]
        public int IdOrderStatus { get; set; }
        public string Name { get; set; } 
    }

}

