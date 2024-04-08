using MotoRental.Core.DTO.NotificationService;
using MotoRental.Core.ResponseDefault;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotoRental.Service.Interface
{
    public interface INotificationService
    {
        Task<ReturnAPI> AddNotificationOrderDeliveryPersonAsync(NotificationOrderDeliveryPersonsDTO notificationOrder);

        /// <summary>
        /// Recupera uma lista de todas as notificações do sistema, incluindo os detalhes das notificações, pedidos associados e entregadores notificados.
        /// Este método consulta o repositório de notificações para obter os detalhes completos de cada notificação, que incluem o momento em que a notificação foi enviada,
        /// os detalhes do pedido relacionado (como identificador do pedido, data de criação, valor da corrida e status do pedido) e informações sobre o entregador notificado
        /// (identificador, nome, CNPJ, data de nascimento, número e tipo de CNH, caminho da imagem da CNH e status de ativação). Após a recuperação bem-sucedida dos dados,
        /// um objeto ReturnAPI é retornado com o código de status HTTP OK e a coleção de DTOs de resposta de notificação, proporcionando uma visão abrangente das notificações
        /// geradas no sistema.
        /// </summary>
        /// <returns>Retorna uma tarefa que, ao ser completada, fornece um objeto ReturnAPI contendo o código de status HTTP OK e a coleção de DTOs de resposta das notificações.</returns>
        Task<ReturnAPI<IEnumerable<NotificationResponseDTO>>> GetNotificationsAsync();

        /// <summary>
        /// Envia uma notificação para um conjunto de entregadores especificados por seus identificadores únicos, utilizando um sistema de mensageria para a distribuição das notificações.
        /// Este método serializa os identificadores dos entregadores e a mensagem de notificação em um objeto JSON, que é então publicado em uma fila do RabbitMQ destinada a notificações.
        /// Após a publicação bem-sucedida da mensagem na fila, o método retorna um objeto ReturnAPI com o código de status HTTP OK e uma mensagem indicando o sucesso do envio.
        /// </summary>
        /// <param name="deliveryPersonIds">Uma coleção contendo os identificadores únicos dos entregadores que devem receber a notificação.</param>
        /// <param name="message">A mensagem de notificação a ser enviada.</param>
        /// <returns>Retorna uma tarefa que, ao ser completada, fornece um objeto ReturnAPI com o código de status HTTP OK e uma mensagem indicando o sucesso do envio da notificação.</returns>
        Task<ReturnAPI> NotifyDeliveryPersonAsync(IEnumerable<int> deliveryPersonIds, int orderid);
    }
}
