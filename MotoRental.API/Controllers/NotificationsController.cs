using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MotoRental.API.ConfigController;
using MotoRental.Core.Cost;
using MotoRental.Core.DTO.NotificationService;
using MotoRental.Core.ResponseDefault;
using MotoRental.Service.Interface;

namespace MotoRental.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationsController : ControllerBaseCustom
    {
        private readonly INotificationService _notificationService;

        public NotificationsController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        /// <summary>
        /// Recupera uma lista de todas as notificações do sistema, incluindo os detalhes das notificações, pedidos associados e entregadores notificados.
        /// Este método consulta o repositório de notificações para obter os detalhes completos de cada notificação, que incluem o momento em que a notificação foi enviada,
        /// os detalhes do pedido relacionado (como identificador do pedido, data de criação, valor da corrida e status do pedido) e informações sobre o entregador notificado
        /// (identificador, nome, CNPJ, data de nascimento, número e tipo de CNH, caminho da imagem da CNH e status de ativação). Após a recuperação bem-sucedida dos dados,
        /// um objeto ReturnAPI é retornado com o código de status HTTP OK e a coleção de DTOs de resposta de notificação, proporcionando uma visão abrangente das notificações
        /// geradas no sistema.
        /// </summary>

        [HttpGet]
        [Authorize(Policy = RoleConst.AdminOnly)]
        public async Task<ActionResult<ReturnAPI<IEnumerable<NotificationResponseDTO>>>> GetNotifications()
        {
            return   StatusCode(await _notificationService.GetNotificationsAsync());
        }

    }
}
