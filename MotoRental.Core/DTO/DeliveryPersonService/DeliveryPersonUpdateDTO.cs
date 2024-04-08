using Microsoft.AspNetCore.Http;
using MotoRental.Core.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotoRental.Core.DTO.DeliveryPersonService
{
    public class DeliveryPersonUpdateDTO
    {
        public string Name { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string CNHNumber { get; set; }
        public CNHTypeEnum? CNHType { get; set; }
    }

}
