using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MotoRental.API.ConfigController;
using MotoRental.Core.Cost;
using MotoRental.Core.DTO.MotoService;
using MotoRental.Core.ResponseDefault;
using MotoRental.Service.Interface;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MotoRental.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MotosController : ControllerBaseCustom
    {
        private readonly IMotoService _motoService;

        public MotosController(IMotoService motoService)
        {
            _motoService = motoService;
        }

        /// <summary>
        /// Adiciona uma nova motocicleta ao sistema. Este método verifica primeiro se já existe uma motocicleta com a placa fornecida.
        /// Se uma motocicleta com a mesma placa já existir, lança uma exceção BadRequestException. Caso contrário, procede à criação de uma nova entidade de motocicleta
        /// com os dados fornecidos, como ano, modelo e placa, marcando-a como ativa. Após a criação bem-sucedida da entidade, as informações da motocicleta são mapeadas
        /// para um DTO de resposta e um objeto ReturnAPI é retornado com o código de status HTTP Created, junto com o DTO de resposta, indicando o sucesso na adição da motocicleta.
        /// Uma mensagem de log é também registrada para informar a adição da nova motocicleta.
        /// </summary>
        [HttpPost]
        [Authorize(Policy = RoleConst.AdminOnly)]
        public async Task<ActionResult<ReturnAPI<MotoResponseDTO>>> AddMoto([FromBody] MotoCreateDTO motoDto)
        {
            return StatusCode(await _motoService.AddMotoAsync(motoDto));
        }

        /// <summary>
        /// Recupera uma lista de todas as motocicletas registradas no sistema, opcionalmente filtrada por placa de licença. Este método oferece a flexibilidade de recuperar todas as motocicletas ou apenas aquelas que correspondem a uma placa de licença específica. 
        /// A busca é ordenada pelo ano da motocicleta. Após recuperar as motocicletas conforme o filtro aplicado, os dados são mapeados para uma coleção de DTOs de resposta, incluindo informações como identificador, ano, modelo, placa de licença e status de ativação. 
        /// Finalmente, um objeto ReturnAPI é retornado com o código de status HTTP OK e a coleção de DTOs de resposta, indicando o sucesso da operação de recuperação.
        /// </summary>
        [HttpGet]
        [Authorize(Policy = RoleConst.AdminOrEntregador)]
        public async Task<ActionResult<ReturnAPI<IEnumerable<MotoResponseDTO>>>> GetAllMotos([FromQuery] string licensePlate = null)
        {
            return StatusCode(await _motoService.GetAllMotosAsync(licensePlate));
        }

        /// <summary>
        /// Busca por uma motocicleta específica no sistema usando seu identificador único, garantindo que esta esteja ativa. Este método tenta localizar a motocicleta no repositório correspondente. 
        /// Se a motocicleta não for encontrada ou não estiver ativa, uma exceção NotFoundException é lançada. Caso a motocicleta seja encontrada, suas informações são mapeadas para um DTO de resposta, 
        /// incluindo detalhes como identificador, ano, modelo e placa de licença. Após a localização e mapeamento bem-sucedidos, um objeto ReturnAPI é retornado com o código de status HTTP OK e o DTO de resposta, 
        /// indicando o sucesso da operação de recuperação.
        /// </summary>
        [HttpGet("{id}")]
        [Authorize(Policy = RoleConst.AdminOrEntregador)]
        public async Task<ActionResult<ReturnAPI<MotoResponseDTO>>> GetMotoById([FromRoute]int id)
        {
            return StatusCode(await _motoService.GetMotoByIdAsync(id));
        }


        /// <summary>
        /// Atualiza os dados de uma motocicleta existente no sistema, com base nos novos valores fornecidos. Este método primeiro verifica a existência da motocicleta pelo seu identificador.
        /// Se não for encontrada, lança uma exceção NotFoundException. Em seguida, verifica se a nova placa fornecida já pertence a outra motocicleta, exceto a que está sendo atualizada, e lança
        /// uma BadRequestException se verdadeiro. Os dados da motocicleta, como placa, modelo e ano, são atualizados conforme fornecido no DTO de atualização, se não forem nulos ou strings vazias.
        /// Após a atualização bem-sucedida, o método salva as alterações e retorna um objeto ReturnAPI com o código de status HTTP OK, indicando o sucesso da operação.
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Policy = RoleConst.AdminOnly)]
        public async Task<ActionResult<ReturnAPI>> UpdateMoto([FromRoute] int id, [FromBody] MotoUpdateDTO motoDto)
        {
            return StatusCode(await _motoService.UpdateMotoAsync(id, motoDto));
        }

        /// <summary>
        /// Desativa uma motocicleta no sistema, marcando-a como inativa em vez de remover seus registros permanentemente. Primeiramente, busca-se a motocicleta pelo identificador fornecido.
        /// Se a motocicleta não for encontrada, uma exceção NotFoundException é lançada. Se encontrada, verifica-se se a motocicleta está atualmente alugada. Se estiver alugada, lança-se uma BadRequestException,
        /// impedindo a desativação. Caso a motocicleta não esteja alugada, seu status é alterado para inativo. Após a atualização bem-sucedida, retorna-se um objeto ReturnAPI com o código de status HTTP OK e uma mensagem
        /// indicando o sucesso na desativação da motocicleta.
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Policy = RoleConst.AdminOnly)]
        public async Task<ActionResult<ReturnAPI>> DeleteMoto([FromRoute] int id)
        {
            return StatusCode(await _motoService.DeleteMotoAsync(id));
        }
    }
}
