using MotoRental.Core.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotoRental.Core.DTO.DeliveryPersonService
{
    public class DeliveryPersonCreateDTO
    {
        public string Name { get; set; }
        public string CNPJ { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string CNHNumber { get; set; }
        public CNHTypeEnum CNHType { get; set; } // Exemplo: "A", "B", ou "A+B"
                                            // Considerar incluir um campo para o arquivo da CNH, dependendo de como você pretende receber este arquivo (por exemplo, como um base64 em um campo de string).
    }
}
