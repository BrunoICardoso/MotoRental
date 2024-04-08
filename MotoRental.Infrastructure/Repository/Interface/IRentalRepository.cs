using MotoRental.Core.DTO.RentalService;
using MotoRental.Core.Entity.SchemaCore;
using MotoRental.Core.ResponseDefault;
using MotoRental.Infrastructure.Repository.Settings.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotoRental.Infrastructure.Repository.Interface
{
    public interface IRentalRepository : IDomainRepository<Rental>
    {
        Task<(IEnumerable<RentalResponseDTO> result, int total)> GetAllRentalDetailsAsync(FilterPage model);
        Task<RentalResponseDTO> GetRentalDetailsAsync(int rentalId);
        Task<bool> IsAnyRentalActiveForDeliveryPerson(int deliveryPersonId);
        Task<bool> IsAnyRentalActiveForMoto(int motoId);
    }
}
