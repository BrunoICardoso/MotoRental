using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotoRental.Core.Entity.SchemaCore
{
    [Table($"{nameof(DeliveryPerson)}", Schema = "core")]
    public class DeliveryPerson
    {
        [Key]
        public int IdDeliveryPerson { get; set; }
        public int IdCNHType { get; set; }
        public int IdUser { get; set; }
        public string Name { get; set; } 
        public string CNPJ { get; set; } 
        public DateTime BirthDate { get; set; }
        public string CNHNumber { get; set; }
        public string CNHImagePath { get; set; }
        public bool Active { get; set; }
    }

}
