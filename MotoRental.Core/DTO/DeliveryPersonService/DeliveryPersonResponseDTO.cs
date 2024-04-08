using MotoRental.Core.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotoRental.Core.DTO.DeliveryPersonService
{
    public class DeliveryPersonResponseDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string CNPJ { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string CNHNumber { get; set; }
        public CNHTypeEnum CNHType { get; set; }
        public string CNHImagePath { get; set; }
        public bool Active { get; set; }
    }

}
