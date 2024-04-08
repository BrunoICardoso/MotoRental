using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MotoRental.Core.Entity.SchemaAuth
{
    [Table($"{nameof(User)}", Schema = "auth")]
    public class User
    {
        [Key]
        public int IdUser { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public bool Active { get; set; }
    }
}
