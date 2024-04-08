using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MotoRental.Core.Entity.SchemaAuth
{
    [Table($"{nameof(Role)}", Schema = "auth")]
    public class Role
    {
        [Key]
        public int IdRole { get; set; }
        public string Name { get; set; }
    }
}
