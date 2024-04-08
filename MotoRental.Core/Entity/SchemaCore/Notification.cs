using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotoRental.Core.Entity.SchemaCore
{
    [Table($"{nameof(Notification)}", Schema = "core")]
    public class Notification
    {
        [Key]
        public int IdNotification { get; set; }
        public int IdDeliveryPerson { get; set; }
        public int IdOrder { get; set; }
        public DateTime NotifiedAt { get; set; }
    }
}
