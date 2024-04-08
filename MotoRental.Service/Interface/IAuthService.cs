using MotoRental.Core.DTO.AuthService;
using MotoRental.Core.ResponseDefault;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotoRental.Service.Interface
{
    public interface IAuthService
    {
        /// <summary>
        /// Adiciona um novo usuário ao sistema de maneira assíncrona. Valida inicialmente a senha segundo critérios específicos, exigindo pelo menos
        /// uma letra maiúscula e um caractere especial, além de um comprimento mínimo de 6 caracteres. Se a senha não satisfazer esses requisitos, uma
        /// exceção BadRequestException é lançada, detalhando os critérios não atendidos. Após a validação bem-sucedida da senha, procede-se à criação do
        /// usuário, definindo-o como ativo e atribuindo-lhe a função de administrador. O processo conclui com a geração de um código de status HTTP Created,
        /// indicando a criação bem-sucedida do usuário, e uma mensagem correspondente.
        /// </summary>
        /// <param name="userCreate">Contém os dados necessários para a criação do usuário, incluindo o nome de usuário e a senha.</param>
        /// <returns>Retorna uma tarefa que, ao ser completada, fornece um objeto ReturnAPI com o código de status HTTP Created e uma mensagem indicativa
        /// da criação bem-sucedida do usuário.</returns>
        /// <exception cref="BadRequestException">Esta exceção é lançada caso a senha fornecida não cumpra com os critérios estabelecidos de validação.</exception>

        Task<ReturnAPI> AddUserAsync(UserCreateDTO userCreate);

        /// <summary>
        /// Autentica um usuário com base nas credenciais de login fornecidas. Se não existirem usuários no sistema,
        /// cria um usuário admin com as credenciais fornecidas, desde que a senha atenda a critérios especificados.
        /// Caso contrário, tenta autenticar o usuário com os registros existentes.
        /// </summary>
        /// <param name="login">As credenciais de login contendo o nome de usuário e a senha.</param>
        /// <returns>Uma tarefa que representa a operação assíncrona. O resultado da tarefa contém um <see cref="AuthenticationDTO"/>
        /// com o token JWT gerado, a data de expiração do token, as roles do usuário e o ID do usuário se a autenticação for bem-sucedida.
        /// Lança <see cref="BadRequestException"/> se a senha não atender aos critérios especificados na criação do primeiro usuário.
        /// Lança <see cref="NotFoundException"/> se o usuário não for encontrado ou a senha for inválida para logins subsequentes.</returns>
        /// <exception cref="BadRequestException">Lançada quando a senha fornecida para a criação do admin não atende aos critérios necessários.</exception>
        /// <exception cref="NotFoundException">Lançada quando uma tentativa de login falha devido a um usuário inexistente ou senha incorreta.</exception>
        Task<ReturnAPI<AuthenticationDTO>> AuthenticateAsync(LoginDTO login);

        /// <summary>
        /// Desativa um usuário existente no sistema de maneira assíncrona, marcando-o como inativo em vez de remover seus dados permanentemente.
        /// Esse método busca primeiro o usuário pelo identificador único fornecido. Se o usuário não for encontrado, uma exceção NotFoundException é lançada.
        /// Caso contrário, o status de ativação do usuário é alterado para falso, indicando que o usuário foi desativado. Após a atualização bem-sucedida,
        /// um objeto ReturnAPI é retornado com o código de status HTTP OK e uma mensagem indicando o sucesso da operação de desativação.
        /// </summary>
        /// <param name="userId">O identificador único do usuário a ser desativado.</param>
        /// <returns>Retorna uma tarefa que, ao ser completada, fornece um objeto ReturnAPI com o código de status HTTP OK e uma mensagem de sucesso.</returns>
        /// <exception cref="NotFoundException">Lançada se o usuário especificado não for encontrado.</exception>
        Task<ReturnAPI> DeleteUserAsync(int userId);

        /// <summary>
        /// Recupera uma lista de todos os usuários do sistema, opcionalmente filtrada por identificador de usuário ou nome de usuário.
        /// Este método oferece flexibilidade ao permitir a recuperação de usuários sem filtro ou com base em critérios específicos,
        /// facilitando a busca por um usuário específico ou um conjunto de usuários. Após a obtenção dos usuários conforme os critérios fornecidos,
        /// os dados são encapsulados em um objeto ReturnAPI junto com o código de status HTTP OK, indicando a recuperação bem-sucedida dos dados dos usuários.
        /// </summary>
        /// <param name="userId">Opcional. O identificador único do usuário a ser filtrado. Se nulo, todos os usuários são considerados no resultado.</param>
        /// <param name="username">Opcional. O nome de usuário a ser filtrado. Se nulo, todos os usuários são considerados no resultado.</param>
        /// <returns>Uma tarefa que, ao ser completada, retorna um objeto ReturnAPI contendo o código de status HTTP OK e a lista de usuários (como UserDTOs) que atendem aos critérios de filtragem.</returns>
        Task<ReturnAPI<IEnumerable<UserDTO>>> GetAllUsersAsync(int? userId = null, string username = null);

        /// <summary>
        /// Atualiza as informações de um usuário existente no sistema de maneira assíncrona. Este método permite a atualização do nome de usuário,
        /// da senha, do status de ativação e da função do usuário. A senha é validada conforme critérios específicos antes da atualização. Se o nome
        /// de usuário fornecido já estiver em uso ou a senha não atender aos critérios de validação, uma exceção é lançada. Após a validação bem-sucedida,
        /// os detalhes do usuário são atualizados conforme as informações fornecidas. Se uma nova função é especificada, a função anterior do usuário
        /// é removida e a nova função é atribuída. O método retorna um objeto ReturnAPI com o código de status HTTP OK e uma mensagem de sucesso após a
        /// atualização bem-sucedida do usuário.
        /// </summary>
        /// <param name="userId">O identificador único do usuário a ser atualizado.</param>
        /// <param name="userUpdate">Um DTO contendo as novas informações para atualização do usuário.</param>
        /// <returns>Retorna uma tarefa que, ao ser completada, fornece um objeto ReturnAPI com o código de status HTTP OK e uma mensagem indicativa da
        /// atualização bem-sucedida do usuário.</returns>
        /// <exception cref="NotFoundException">Lançada se o usuário especificado não for encontrado.</exception>
        /// <exception cref="BadRequestException">Lançada se o novo nome de usuário já estiver em uso ou se a nova senha não atender aos critérios de validação.</exception>
        Task<ReturnAPI> UpdateUserAsync(int userId, UserUpdateDTO userUpdate);
    }
}
