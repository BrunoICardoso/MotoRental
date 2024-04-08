using MotoRental.Core.DTO.OrderService;
using MotoRental.Core.ResponseDefault;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotoRental.Service.Interface
{
    public interface IOrderService
    {
        /// <summary>
        /// Atualiza o status de um pedido para "Aceito" por um entregador específico, verificando se o entregador e o pedido existem, se o entregador possui um aluguel ativo e se o entregador foi notificado sobre o pedido.
        /// Primeiro, confirma a existência do entregador e do pedido nos respectivos repositórios. Se algum não for encontrado, lança uma exceção NotFoundException. Em seguida, verifica se o entregador possui um aluguel ativo;
        /// caso contrário, lança uma BadRequestException indicando a ausência de um aluguel ativo. Também verifica se existe uma notificação para o entregador sobre o pedido em questão. Se confirmado, o status do pedido é atualizado
        /// para "Aceito", e o sistema salva a alteração. Caso o entregador não tenha sido notificado sobre o pedido, uma BadRequestException é lançada. Se todas as verificações forem bem-sucedidas e o pedido for atualizado,
        /// retorna um objeto ReturnAPI com o código de status HTTP OK e uma mensagem indicando o sucesso da aceitação do pedido.
        /// </summary>
        /// <param name="orderId">O identificador único do pedido a ser atualizado.</param>
        /// <param name="deliveryPersonId">O identificador único do entregador que está aceitando o pedido.</param>
        /// <returns>Retorna uma tarefa que, ao ser completada, fornece um objeto ReturnAPI com o código de status HTTP OK e uma mensagem de sucesso.</returns>
        /// <exception cref="NotFoundException">Lançada se o entregador ou o pedido especificado não for encontrado.</exception>
        /// <exception cref="BadRequestException">Lançada se o entregador não possuir um aluguel ativo ou não tiver sido notificado sobre o pedido.</exception
        Task<ReturnAPI> UpdateAcceptOrderAsync(int orderId, int deliveryPersonId);

        /// <summary>
        /// Cria um novo pedido no sistema e notifica os entregadores elegíveis para a realização do mesmo. Primeiramente, um novo pedido é criado com o valor da corrida fornecido e a data atual como data de criação,
        /// sendo inicialmente marcado com o status "Disponível". Após a criação do pedido no repositório, o sistema busca por entregadores elegíveis para realizar o pedido. Esses entregadores são então notificados
        /// sobre o novo pedido disponível. Finalmente, os detalhes do pedido criado são recuperados e retornados em um DTO de resposta, juntamente com um código de status HTTP Created, indicando o sucesso da criação
        /// e notificação do pedido.
        /// </summary>
        /// <param name="orderDto">Um DTO contendo os dados necessários para a criação do pedido, incluindo o valor da corrida.</param>
        /// <returns>Retorna uma tarefa que, ao ser completada, fornece um objeto ReturnAPI contendo o código de status HTTP Created e o DTO de resposta com os detalhes do pedido criado.</returns>
        /// <exception cref="Exception">Lançada em caso de qualquer falha durante o processo de criação do pedido ou notificação dos entregadores.</exception>
        Task<ReturnAPI<OrderResponseDTO>> AddOrderAsync(OrderCreateDTO orderDto);

        /// <summary>
        /// Atualiza o status de um pedido para "Entregue" por um entregador específico. Inicialmente, verifica se o pedido existe e está no estado "Aceito".
        /// Se o pedido não existir ou não estiver no estado correto, lança uma exceção NotFoundException. Além disso, verifica se o entregador possui um aluguel ativo;
        /// se não, uma BadRequestException é lançada indicando a ausência de um aluguel ativo. Após passar por essas verificações, o status do pedido é atualizado para "Entregue".
        /// As alterações são então salvas, e o método retorna um objeto ReturnAPI com o código de status HTTP OK e uma mensagem indicando o sucesso na conclusão do pedido.
        /// </summary>
        /// <param name="orderId">O identificador único do pedido a ser atualizado para "Entregue".</param>
        /// <param name="deliveryPersonId">O identificador único do entregador que está completando o pedido.</param>
        /// <returns>Retorna uma tarefa que, ao ser completada, fornece um objeto ReturnAPI com o código de status HTTP OK e uma mensagem de sucesso na conclusão do pedido.</returns>
        /// <exception cref="NotFoundException">Lançada se o pedido especificado não for encontrado ou não estiver em um estado que permita a atualização para "Entregue".</exception>
        /// <exception cref="BadRequestException">Lançada se o entregador não possuir um aluguel ativo.</exception>
        Task<ReturnAPI> UpdateCompleteOrderAsync(int orderId, int deliveryPersonId);
    }
}
