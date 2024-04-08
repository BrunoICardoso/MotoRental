using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotoRental.Core.DTO.MotoService
{
    public class MotoUpdateDTO
    {
        public int? Year { get; set; }
        public string Model { get; set; }
        public string LicensePlate { get; set; }
    }
}
