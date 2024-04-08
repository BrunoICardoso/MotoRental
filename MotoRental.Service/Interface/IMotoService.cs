using MotoRental.Core.DTO.MotoService;
using MotoRental.Core.ResponseDefault;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotoRental.Service.Interface
{
    public interface IMotoService
    {
        /// <summary>
        /// Adiciona uma nova motocicleta ao sistema. Este método verifica primeiro se já existe uma motocicleta com a placa fornecida.
        /// Se uma motocicleta com a mesma placa já existir, lança uma exceção BadRequestException. Caso contrário, procede à criação de uma nova entidade de motocicleta
        /// com os dados fornecidos, como ano, modelo e placa, marcando-a como ativa. Após a criação bem-sucedida da entidade, as informações da motocicleta são mapeadas
        /// para um DTO de resposta e um objeto ReturnAPI é retornado com o código de status HTTP Created, junto com o DTO de resposta, indicando o sucesso na adição da motocicleta.
        /// Uma mensagem de log é também registrada para informar a adição da nova motocicleta.
        /// </summary>
        /// <param name="motoDto">Um DTO contendo os dados necessários para criar uma nova motocicleta, incluindo ano, modelo e placa.</param>
        /// <returns>Retorna uma tarefa que, ao ser completada, fornece um objeto ReturnAPI contendo o código de status HTTP Created e o DTO de resposta da motocicleta adicionada.</returns>
        /// <exception cref="BadRequestException">Lançada se uma motocicleta com a mesma placa já existir no sistema.</exception>
        Task<ReturnAPI<MotoResponseDTO>> AddMotoAsync(MotoCreateDTO motoDto);

        /// <summary>
        /// Desativa uma motocicleta no sistema, marcando-a como inativa em vez de remover seus registros permanentemente. Primeiramente, busca-se a motocicleta pelo identificador fornecido.
        /// Se a motocicleta não for encontrada, uma exceção NotFoundException é lançada. Se encontrada, verifica-se se a motocicleta está atualmente alugada. Se estiver alugada, lança-se uma BadRequestException,
        /// impedindo a desativação. Caso a motocicleta não esteja alugada, seu status é alterado para inativo. Após a atualização bem-sucedida, retorna-se um objeto ReturnAPI com o código de status HTTP OK e uma mensagem
        /// indicando o sucesso na desativação da motocicleta.
        /// </summary>
        /// <param name="id">O identificador único da motocicleta a ser desativada.</param>
        /// <returns>Retorna uma tarefa que, ao ser completada, fornece um objeto ReturnAPI com o código de status HTTP OK e uma mensagem de sucesso.</returns>
        /// <exception cref="NotFoundException">Lançada se a motocicleta especificada não for encontrada.</exception>
        /// <exception cref="BadRequestException">Lançada se a motocicleta estiver atualmente alugada e não puder ser desativada.</exception>
        Task<ReturnAPI> DeleteMotoAsync(int id);

        /// <summary>
        /// Recupera uma lista de todas as motocicletas registradas no sistema, opcionalmente filtrada por placa de licença. Este método oferece a flexibilidade de recuperar todas as motocicletas ou apenas aquelas que correspondem a uma placa de licença específica. 
        /// A busca é ordenada pelo ano da motocicleta. Após recuperar as motocicletas conforme o filtro aplicado, os dados são mapeados para uma coleção de DTOs de resposta, incluindo informações como identificador, ano, modelo, placa de licença e status de ativação. 
        /// Finalmente, um objeto ReturnAPI é retornado com o código de status HTTP OK e a coleção de DTOs de resposta, indicando o sucesso da operação de recuperação.
        /// </summary>
        /// <param name="licensePlate">Opcional. A placa de licença a ser usada como filtro na busca por motocicletas. Se não fornecido, todas as motocicletas são consideradas.</param>
        /// <returns>Retorna uma tarefa que, ao ser completada, fornece um objeto ReturnAPI contendo o código de status HTTP OK e a coleção de DTOs de resposta das motocicletas.</returns>
        Task<ReturnAPI<IEnumerable<MotoResponseDTO>>> GetAllMotosAsync(string licensePlate = null);

        /// <summary>
        /// Busca por uma motocicleta específica no sistema usando seu identificador único, garantindo que esta esteja ativa. Este método tenta localizar a motocicleta no repositório correspondente. 
        /// Se a motocicleta não for encontrada ou não estiver ativa, uma exceção NotFoundException é lançada. Caso a motocicleta seja encontrada, suas informações são mapeadas para um DTO de resposta, 
        /// incluindo detalhes como identificador, ano, modelo e placa de licença. Após a localização e mapeamento bem-sucedidos, um objeto ReturnAPI é retornado com o código de status HTTP OK e o DTO de resposta, 
        /// indicando o sucesso da operação de recuperação.
        /// </summary>
        /// <param name="id">O identificador único da motocicleta a ser localizada.</param>
        /// <returns>Retorna uma tarefa que, ao ser completada, fornece um objeto ReturnAPI contendo o código de status HTTP OK e o DTO de resposta da motocicleta encontrada.</returns>
        /// <exception cref="NotFoundException">Lançada se a motocicleta especificada não for encontrada ou não estiver ativa.</exception>
        Task<ReturnAPI<MotoResponseDTO>> GetMotoByIdAsync(int id);

        /// <summary>
        /// Atualiza os dados de uma motocicleta existente no sistema, com base nos novos valores fornecidos. Este método primeiro verifica a existência da motocicleta pelo seu identificador.
        /// Se não for encontrada, lança uma exceção NotFoundException. Em seguida, verifica se a nova placa fornecida já pertence a outra motocicleta, exceto a que está sendo atualizada, e lança
        /// uma BadRequestException se verdadeiro. Os dados da motocicleta, como placa, modelo e ano, são atualizados conforme fornecido no DTO de atualização, se não forem nulos ou strings vazias.
        /// Após a atualização bem-sucedida, o método salva as alterações e retorna um objeto ReturnAPI com o código de status HTTP OK, indicando o sucesso da operação.
        /// </summary>
        /// <param name="id">O identificador único da motocicleta a ser atualizada.</param>
        /// <param name="motoDto">Um DTO contendo os novos dados para atualização da motocicleta.</param>
        /// <returns>Retorna uma tarefa que, ao ser completada, fornece um objeto ReturnAPI com o código de status HTTP OK e uma mensagem de sucesso.</returns>
        /// <exception cref="NotFoundException">Lançada se a motocicleta especificada não for encontrada.</exception>
        /// <exception cref="BadRequestException">Lançada se a nova placa fornecida já estiver em uso por outra motocicleta.</exception>
        Task<ReturnAPI> UpdateMotoAsync(int id, MotoUpdateDTO motoDto);
    }
}
