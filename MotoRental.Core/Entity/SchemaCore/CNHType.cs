using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotoRental.Core.Entity.SchemaCore
{
    [Table($"{nameof(CNHType)}", Schema = "core")]
    public class CNHType
    {
        [Key]
        public int IdCNHType { get; set; } 
        public string Name { get; set; } 
    }

}
