using BurgerRoyale.Domain.ResponseDefault;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MotoRental.API.ConfigController;
using MotoRental.Core.Cost;
using MotoRental.Core.DTO.RentalService;
using MotoRental.Core.ResponseDefault;
using MotoRental.Service.Interface;
using System.Threading.Tasks;

namespace MotoRental.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RentalsController : ControllerBaseCustom
    {
        private readonly IRentalService _rentalService;

        public RentalsController(IRentalService rentalService)
        {
            _rentalService = rentalService;
        }

        /// <summary>
        /// Cria um novo aluguel de motocicleta para um entregador, validando a autorização do entregador com base no tipo de CNH e a disponibilidade da moto.
        /// Primeiro, verifica se o entregador existe e se está autorizado a alugar uma moto, de acordo com seu tipo de CNH (A ou AB). Em seguida, verifica se a moto especificada
        /// existe e não está atualmente alugada. Se todas as validações forem bem-sucedidas, o aluguel é registrado com uma data de início e uma data de término esperada, calculada com base
        /// no plano de aluguel selecionado. Após a criação bem-sucedida do registro de aluguel, as informações do aluguel são encapsuladas em um DTO de resposta e retornadas junto com um código
        /// de status HTTP Created, indicando o sucesso da operação.
        /// </summary>
        [HttpPost]
        [Authorize(Policy = RoleConst.AdminOrEntregador)]
        public async Task<ActionResult<ReturnAPI<RentalResponseDTO>>> AddRental(RentalCreateDTO rentalDto)
        {
            return StatusCode(await _rentalService.AddRentalAsync(rentalDto));
        }

        /// <summary>
        /// Processa a devolução de uma motocicleta alugada, calculando o custo total do aluguel com base na data de devolução, na duração do aluguel e no plano de aluguel aplicado. 
        /// Primeiro, busca-se pelo registro de aluguel usando o identificador único fornecido. Se o aluguel não for encontrado, uma exceção NotFoundException é lançada. 
        /// Verifica-se então se a data de devolução é válida em relação à data de início do aluguel. O custo total do aluguel é calculado levando em consideração a duração real do aluguel,
        /// o custo diário estipulado pelo plano e custos adicionais ou descontos aplicáveis por devolução antecipada ou atrasada. Após atualizar o registro de aluguel com a data de devolução 
        /// e o custo total, as informações atualizadas são encapsuladas em um DTO de resposta e retornadas junto com um código de status HTTP OK, indicando o sucesso da operação de devolução.
        /// </summary>
        [HttpPost("{rentalId}/return")]
        [Authorize(Policy = RoleConst.AdminOrEntregador)]
        public async Task<ActionResult<ReturnAPI<RentalResponseDTO>>> ReturnMoto(int rentalId)
        {
            return StatusCode(await _rentalService.UpdateReturnMotoAsync(rentalId));
        }

        /// <summary>
        /// Recupera uma lista de aluguéis ativos com base nos critérios de filtro e paginação fornecidos. Este método consulta o repositório de aluguéis para obter detalhes
        /// dos aluguéis ativos, incluindo identificadores do aluguel, entregador, motocicleta, datas de início e término esperado, e custos diários, aplicando os filtros e a paginação especificados.
        /// As informações recuperadas são encapsuladas em um objeto ReturnAPIDataTable, que além dos dados dos aluguéis, inclui o total de registros correspondentes aos critérios de filtro para auxiliar na paginação.
        /// O método retorna este objeto, proporcionando uma forma estruturada e eficiente de apresentar os dados de aluguéis ativos, facilitando sua exibição em interfaces que suportam paginação e filtragem.
        /// </summary>
        [HttpGet("active")]
        public async Task<ActionResult<ReturnAPIDataTable<IEnumerable<RentalResponseDTO>>>> GetActiveRentals([FromQuery] FilterPage model)
        {
            return StatusCode(await _rentalService.GetActiveRentalsAsync(model));
        }
    }
}
