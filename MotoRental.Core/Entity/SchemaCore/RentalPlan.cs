using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotoRental.Core.Entity.SchemaCore
{
    [Table($"{nameof(RentalPlan)}", Schema = "core")]
    public class RentalPlan
    {
        [Key]
        public int IdRentalPlan { get; set; }
        public int DurationDays { get; set; }
        public decimal DailyCost { get; set; }
        public decimal PenaltyRate { get; set; }
        public decimal AdditionalDailyCost { get; set; }
    }
}
