using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MotoRental.Core.Entity.SchemaCore
{

    [Table($"{nameof(Moto)}", Schema = "core")]
    public class Moto
    {
        [Key]
        public int IdMoto { get; set; }
        public int Year { get; set; } 
        public string Model { get; set; } 
        public string LicensePlate { get; set; } 
        public bool Active { get; set; }
    }
}
