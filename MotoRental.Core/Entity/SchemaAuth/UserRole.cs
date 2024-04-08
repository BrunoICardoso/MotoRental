using System.ComponentModel.DataAnnotations.Schema;
using System.Data;

namespace MotoRental.Core.Entity.SchemaAuth
{
    [Table($"{nameof(UserRole)}", Schema = "auth")]
    public class UserRole
    {
        public int IdUser { get; set; }
        public int IdRole { get; set; }

    }
}
