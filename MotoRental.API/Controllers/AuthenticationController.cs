using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MotoRental.API.ConfigController;
using MotoRental.Core.Cost;
using MotoRental.Core.DTO.AuthService;
using MotoRental.Core.Entity.SchemaAuth;
using MotoRental.Core.Exceptions;
using MotoRental.Core.ResponseDefault;
using MotoRental.Service.Interface;

namespace MotoRental.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBaseCustom
    {
        private readonly IAuthService _authService;

        public AuthenticationController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Autentica um usuário com base nas credenciais de login fornecidas. Se não existirem usuários no sistema,
        /// cria um usuário admin com as credenciais fornecidas, desde que a senha atenda a critérios especificados.
        /// Caso contrário, tenta autenticar o usuário com os registros existentes.
        /// </summary>
        [HttpPost("login")]
        public async Task<ActionResult<AuthenticationDTO>> Authenticate([FromBody] LoginDTO login)
        {
            var authResult = await _authService.AuthenticateAsync(login);
            return StatusCode((int)authResult.StatusCode, authResult.Data);
        }

        /// <summary>
        /// Adiciona um novo usuário ao sistema. Valida inicialmente a senha segundo critérios específicos, exigindo pelo menos
        /// uma letra maiúscula e um caractere especial, além de um comprimento mínimo de 6 caracteres. Se a senha não satisfazer esses requisitos, uma
        /// exceção BadRequestException é lançada, detalhando os critérios não atendidos. Após a validação bem-sucedida da senha, procede-se à criação do
        /// usuário, definindo-o como ativo e atribuindo-lhe a função de administrador. 
        /// </summary>
        [HttpPost]
        [Authorize(Policy = RoleConst.AdminOnly)]
        public async Task<ActionResult<ReturnAPI>> AddUser([FromBody] UserCreateDTO userCreateDTO)
        {
            return StatusCode(await _authService.AddUserAsync(userCreateDTO));
        }

        /// <summary>
        /// Este método permite a atualização do nome de usuário, da senha, do status de ativação e da função do usuário. A senha é validada conforme critérios específicos antes da atualização.
        /// Se o nome do usuário fornecido já estiver em uso ou a senha não atender aos critérios de validação, uma exceção é lançada. Após a validação bem-sucedida,
        /// os detalhes do usuário são atualizados conforme as informações fornecidas. Se uma nova função é especificada, a função anterior do usuário é removida e a nova função é atribuída.
        /// </summary>
        [HttpPut("{userid}")]
        [Authorize(Policy = RoleConst.AdminOnly)]
        public async Task<ActionResult<ReturnAPI>> UpdateUser([FromRoute]int userid, [FromBody] UserUpdateDTO userUpdate)
        {
            return StatusCode(await _authService.UpdateUserAsync(userid,userUpdate));
        }

        /// <summary>
        /// Desativa um usuário existente no sistema de maneira assíncrona, marcando-o como inativo em vez de remover seus dados permanentemente.
        /// Esse método busca primeiro o usuário pelo identificador único fornecido. Se o usuário não for encontrado, uma exceção NotFoundException é lançada.
        /// Caso contrário, o status de ativação do usuário é alterado para falso, indicando que o usuário foi desativado.
        /// </summary>
        [HttpDelete("{userid}")]
        [Authorize(Policy = RoleConst.AdminOnly)]
        public async Task<ActionResult<ReturnAPI>> DeleteUser([FromRoute] int userid)
        {
            return StatusCode(await _authService.DeleteUserAsync(userid));
        }

        /// <summary>
        /// Recupera uma lista de todos os usuários do sistema, opcionalmente filtrada por identificador de usuário ou nome de usuário.
        /// Este método oferece flexibilidade ao permitir a recuperação de usuários sem filtro ou com base em critérios específicos,
        /// facilitando a busca por um usuário específico ou um conjunto de usuários. Após a obtenção dos usuários conforme os critérios fornecidos,
        /// os dados são encapsulados em um objeto ReturnAPI.
        /// </summary>
        [HttpGet]
        [Authorize(Policy = RoleConst.AdminOnly)]
        public async Task<ActionResult<ReturnAPI<IEnumerable<UserDTO>>>> GetallUser([FromQuery] int? userId = null, [FromQuery] string username = null)
        {
            return StatusCode(await _authService.GetAllUsersAsync(userId, username));
        }

    }
}