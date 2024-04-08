using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MotoRental.API.ConfigController;
using MotoRental.Core.Cost;
using MotoRental.Core.DTO.OrderService;
using MotoRental.Core.ResponseDefault;
using MotoRental.Service.Interface;

namespace MotoRental.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBaseCustom
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        /// <summary>
        /// Cria um novo pedido no sistema e notifica os entregadores elegíveis para a realização do mesmo. Primeiramente, um novo pedido é criado com o valor da corrida fornecido e a data atual como data de criação,
        /// sendo inicialmente marcado com o status "Disponível". Após a criação do pedido no repositório, o sistema busca por entregadores elegíveis para realizar o pedido. Esses entregadores são então notificados
        /// sobre o novo pedido disponível. Finalmente, os detalhes do pedido criado são recuperados e retornados em um DTO de resposta, juntamente com um código de status HTTP Created, indicando o sucesso da criação
        /// e notificação do pedido.
        /// </summary>
        [HttpPost]
        [Authorize(Policy = RoleConst.AdminOnly)]
        public async Task<ActionResult<ReturnAPI<OrderResponseDTO>>> AddOrder([FromBody] OrderCreateDTO orderDto)
        {
            return StatusCode(await _orderService.AddOrderAsync(orderDto));
        }

        /// <summary>
        /// Atualiza o status de um pedido para "Aceito" por um entregador específico, verificando se o entregador e o pedido existem, se o entregador possui um aluguel ativo e se o entregador foi notificado sobre o pedido.
        /// Primeiro, confirma a existência do entregador e do pedido nos respectivos repositórios. Se algum não for encontrado, lança uma exceção NotFoundException. Em seguida, verifica se o entregador possui um aluguel ativo;
        /// caso contrário, lança uma BadRequestException indicando a ausência de um aluguel ativo. Também verifica se existe uma notificação para o entregador sobre o pedido em questão. Se confirmado, o status do pedido é atualizado
        /// para "Aceito", e o sistema salva a alteração. Caso o entregador não tenha sido notificado sobre o pedido, uma BadRequestException é lançada. Se todas as verificações forem bem-sucedidas e o pedido for atualizado,
        /// retorna um objeto ReturnAPI com o código de status HTTP OK e uma mensagem indicando o sucesso da aceitação do pedido.
        /// </summary>
        [HttpPut("Accept/{orderId}/{deliveryPersonId}")]
        [Authorize(Policy = RoleConst.AdminOrEntregador)]
        public async Task<ActionResult<ReturnAPI>> UpdateAcceptOrder([FromRoute]int orderId, [FromRoute] int deliveryPersonId)
        {
            return StatusCode(await _orderService.UpdateAcceptOrderAsync(orderId, deliveryPersonId));
        }

        /// <summary>
        /// Atualiza o status de um pedido para "Entregue" por um entregador específico. Inicialmente, verifica se o pedido existe e está no estado "Aceito".
        /// Se o pedido não existir ou não estiver no estado correto, lança uma exceção NotFoundException. Além disso, verifica se o entregador possui um aluguel ativo;
        /// se não, uma BadRequestException é lançada indicando a ausência de um aluguel ativo. Após passar por essas verificações, o status do pedido é atualizado para "Entregue".
        /// As alterações são então salvas, e o método retorna um objeto ReturnAPI com o código de status HTTP OK e uma mensagem indicando o sucesso na conclusão do pedido.
        /// </summary>
        [HttpPut("Complete/{orderId}/{deliveryPersonId}")]
        [Authorize(Policy = RoleConst.AdminOrEntregador)]
        public async Task<ActionResult<ReturnAPI>> UpdateCompleteOrder([FromRoute]int orderId, [FromRoute] int deliveryPersonId)
        {
            return StatusCode(await _orderService.UpdateCompleteOrderAsync(orderId, deliveryPersonId));
        }


    }
}
