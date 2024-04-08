using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MotoRental.API.ConfigController;
using MotoRental.Core.Cost;
using MotoRental.Core.DTO.AuthService;
using MotoRental.Core.DTO.DeliveryPersonService;
using MotoRental.Core.ResponseDefault;
using MotoRental.Service.Interface;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BurgerRoyale.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeliveryPersonsController : ControllerBaseCustom
    {
        private readonly IDeliveryPersonService _deliveryPersonService;
        private readonly JwtTokenDTO _jwtTokenDTO;

        public DeliveryPersonsController(IDeliveryPersonService deliveryPersonService, JwtTokenDTO jwtTokenDTO)
        {
            _deliveryPersonService = deliveryPersonService;
            _jwtTokenDTO = jwtTokenDTO;
        }

        /// <summary>
        /// Adiciona um novo entregador ao sistema. Este método valida se o CNPJ fornecido já está em uso;
        /// se estiver, lança uma exceção BadRequestException. Se o CNPJ estiver disponível, cria uma nova instância de entregador
        /// com os dados fornecidos no DTO de criação e associa este entregador ao usuário indicado pelo identificador fornecido.
        /// Após a criação bem-sucedida do entregador, um DTO de resposta é preparado com os detalhes do entregador criado, incluindo
        /// seu identificador no sistema, nome, CNPJ, número e tipo de CNH, e data de nascimento. O método retorna um objeto ReturnAPI
        /// contendo o DTO de resposta, indicando o sucesso da operação de adição do entregador.
        /// </summary>
        [HttpPost]
        [Authorize(Policy = RoleConst.EntregadorOnly)]
        public async Task<ActionResult<ReturnAPI<DeliveryPersonResponseDTO>>> AddDeliveryPerson([FromBody] DeliveryPersonCreateDTO deliveryPersonDto)
        {
            var result = await _deliveryPersonService.AddDeliveryPersonAsync(_jwtTokenDTO.IdUser, deliveryPersonDto);
            return StatusCode(result);
        }


        /// <summary>
        /// Atualiza o arquivo da CNH de um entregador específico de forma assíncrona. Este método primeiro verifica se o entregador existe
        /// no sistema usando o identificador fornecido. Se o entregador não for encontrado, lança uma exceção NotFoundException. Caso o entregador
        /// seja localizado, o arquivo da CNH fornecido é salvo em um diretório especificado com um nome de arquivo único, e o caminho do arquivo
        /// é atualizado no registro do entregador. Após a atualização bem-sucedida do caminho do arquivo da CNH no registro do entregador, o método
        /// retorna um objeto ReturnAPI com o código de status HTTP OK e uma mensagem indicando que a CNH foi atualizada com sucesso.
        /// </summary>
        [HttpPut("{id}/cnh")]
        [Authorize(Policy = RoleConst.AdminOrEntregador)]
        public async Task<ActionResult<ReturnAPI>> UpdateCnh(int id, [FromForm] UpdateCNHFileDTO model)
        {
            var result = await _deliveryPersonService.UpdateCnhAsync(id, model);
            return StatusCode(result);
        }


        /// <summary>
        /// Atualiza as informações de um entregador específico no sistema. Verifica primeiro se o entregador existe
        /// usando o identificador fornecido. Se o entregador não for encontrado, lança uma exceção NotFoundException. Se encontrado, as informações
        /// do entregador, como nome, data de nascimento, número e tipo de CNH, são atualizadas conforme os dados fornecidos no DTO de atualização.
        /// Se uma nova imagem de CNH for fornecida, o arquivo é salvo com um nome de arquivo único no local especificado, e o caminho para este arquivo
        /// é atualizado no registro do entregador. Após a atualização bem-sucedida das informações do entregador, o método retorna um objeto ReturnAPI
        /// com o código de status HTTP OK e uma mensagem indicando o sucesso da atualização.
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Policy = RoleConst.AdminOrEntregador)]
        public async Task<ActionResult<ReturnAPI>> UpdateDeliveryPerson([FromRoute]int id, [FromBody] DeliveryPersonUpdateDTO deliveryPersonDto)
        {
            var result = await _deliveryPersonService.UpdateDeliveryPersonAsync(id, deliveryPersonDto);
            return StatusCode(result);
        }

        /// <summary>
        /// Recupera uma lista de todos os entregadores registrados no sistema. Este método acessa o repositório de entregadores,
        /// obtém todos os registros de entregadores, e os mapeia para uma coleção de DTOs de resposta que incluem detalhes como identificador, nome, CNPJ,
        /// caminho da imagem da CNH, número da CNH, tipo da CNH, data de nascimento e status de ativação. Após a recuperação e mapeamento bem-sucedidos,
        /// um objeto ReturnAPI é retornado com o código de status HTTP OK e a coleção de DTOs de resposta, indicando o sucesso da operação de recuperação.
        /// </summary>
        [HttpGet]
        [Authorize(Policy = RoleConst.AdminOrEntregador)]
        public async Task<ActionResult<ReturnAPI<IEnumerable<DeliveryPersonResponseDTO>>>> GetAllDeliveryPersons()
        {
            var result = await _deliveryPersonService.GetAllDeliveryPersonsAsync();
            return StatusCode(result);
        }

        /// <summary>
        /// Busca por um entregador específico no sistema usando seu identificador único de forma assíncrona. Este método tenta localizar o entregador
        /// no repositório correspondente. Se o entregador não for encontrado, lança uma exceção NotFoundException. Se encontrado, as informações do
        /// entregador são mapeadas para um DTO de resposta, que inclui detalhes como identificador, nome, CNPJ, caminho da imagem da CNH, número da CNH,
        /// tipo da CNH, data de nascimento e status de ativação. Após a localização e mapeamento bem-sucedidos, um objeto ReturnAPI é retornado com o
        /// código de status HTTP OK e o DTO de resposta, indicando o sucesso da operação de recuperação.
        /// </summary>
        [HttpGet("{id}")]
        [Authorize(Policy = RoleConst.AdminOrEntregador)]
        public async Task<ActionResult<ReturnAPI<DeliveryPersonResponseDTO>>> GetDeliveryPersonById([FromRoute] int id)
        {
            var result = await _deliveryPersonService.GetDeliveryPersonByIdAsync(id);
            return StatusCode(result);
        }


        /// <summary>
        /// Desativa um entregador existente no sistema, marcando-o como inativo em vez de remover seus registros permanentemente.
        /// Primeiro, este método busca pelo entregador usando o identificador único fornecido. Se o entregador não for encontrado, uma exceção NotFoundException
        /// é lançada. Caso o entregador seja localizado, seu status de ativação é alterado para falso, indicando sua desativação no sistema. Após a atualização
        /// bem-sucedida do status do entregador, o método retorna um objeto ReturnAPI com o código de status HTTP OK e uma mensagem indicando o sucesso da
        /// operação de desativação.
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Policy = RoleConst.AdminOrEntregador)]
        public async Task<ActionResult<ReturnAPI>> DeleteDeliveryPerson([FromRoute] int id)
        {
            var result = await _deliveryPersonService.DeleteDeliveryPersonAsync(id);
            return StatusCode(result);
        }
    }
}
