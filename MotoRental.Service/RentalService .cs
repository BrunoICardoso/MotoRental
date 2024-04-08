using BurgerRoyale.Domain.ResponseDefault;
using MotoRental.Core.DTO.RentalService;
using MotoRental.Core.Entity;
using MotoRental.Core.Entity.SchemaCore;
using MotoRental.Core.Enum;
using MotoRental.Core.Exceptions;
using MotoRental.Core.ResponseDefault;
using MotoRental.Infrastructure.Repository;
using MotoRental.Infrastructure.Repository.Interface;
using MotoRental.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MotoRental.Service
{
    public class RentalService : IRentalService
    {
        private readonly IRentalRepository _rentalRepository;
        private readonly IMotoRepository _motoRepository;
        private readonly IDeliveryPersonRepository _deliveryPersonRepository;
        private readonly IRentalPlanRepository _rentalplanRepository;

        public RentalService(IRentalRepository rentalRepository, IMotoRepository motoRepository, IDeliveryPersonRepository deliveryPersonRepository, IRentalPlanRepository rentalplanRepository)
        {
            _rentalRepository = rentalRepository;
            _motoRepository = motoRepository;
            _deliveryPersonRepository = deliveryPersonRepository;
            _rentalplanRepository = rentalplanRepository;
        }

        public async Task<ReturnAPI<RentalResponseDTO>> AddRentalAsync(RentalCreateDTO rentalDto)
        {
            try
            {
                var deliveryPerson = await _deliveryPersonRepository.GetByIdAsync(w => w.IdDeliveryPerson == rentalDto.DeliveryPersonId);
                if (deliveryPerson == null)
                {
                    throw new NotFoundException("Delivery person not found.");
                }

                if (deliveryPerson.IdCNHType != (int)CNHTypeEnum.A && deliveryPerson.IdCNHType != (int)CNHTypeEnum.AB)
                {
                    throw new BadRequestException("Delivery person is not authorized to rent a moto due to CNH type.");
                }

                var moto = await _motoRepository.GetByIdAsync(w => w.IdMoto == rentalDto.MotoId);
                if (moto == null)
                {
                    throw new NotFoundException("Moto not found.");
                }

                var activeRental = await _rentalRepository.IsAnyRentalActiveForMoto(rentalDto.MotoId);
                if (activeRental)
                {
                    throw new BadRequestException("Moto is currently rented.");
                }

                var startDate = rentalDto.StartDate.AddDays(1);

                var rentalplanEntity = await _rentalplanRepository.GetByIdAsync(w => w.DurationDays == (int)rentalDto.Plan);

                var rental = new Rental
                {
                    IdDeliveryPerson = rentalDto.DeliveryPersonId,
                    IdMoto = rentalDto.MotoId,
                    IdRentalPlan = rentalplanEntity.IdRentalPlan,
                    StartDate = startDate,
                    ExpectedEndDate = startDate.AddDays(rentalplanEntity.DurationDays),
                    EndDate = null,
                    DailyCost = rentalplanEntity.DailyCost
                };

                await _rentalRepository.AddAsync(rental);
                await _rentalRepository.SaveChangesAsync();

                var responseDto = await _rentalRepository.GetRentalDetailsAsync(rental.IdRental);

                return new ReturnAPI<RentalResponseDTO>(HttpStatusCode.Created, responseDto);
            }
            catch (Exception)
            {
                _rentalRepository.Rollback();
                throw;
            }

        }


        public async Task<ReturnAPI<RentalResponseDTO>> UpdateReturnMotoAsync(int rentalId)
        {
            try
            {
                var rental = await _rentalRepository.GetByIdAsync(w => w.IdRental == rentalId);
                if (rental == null)
                {
                    throw new NotFoundException("Rental not found.");
                }

                var returnDate = DateTime.Now;
                if (returnDate < rental.StartDate.Date)
                {
                    throw new BadRequestException("Return date cannot be before the rental start date.");
                }

                var rentalPlan = await _rentalplanRepository.GetByIdAsync(w => w.IdRentalPlan == rental.IdRentalPlan);

                if (rentalPlan == null)
                {
                    throw new Exception("Rental plan not found.");
                }

                int daysRented = (returnDate - rental.StartDate).Days;
                decimal totalCost = daysRented * rentalPlan.DailyCost;

                if (daysRented > rentalPlan.DurationDays)
                {
                    totalCost += (daysRented - rentalPlan.DurationDays) * rentalPlan.AdditionalDailyCost;
                }
                else if (daysRented < rentalPlan.DurationDays)
                {
                    int daysEarly = rentalPlan.DurationDays - daysRented;
                    decimal discount = daysEarly * rentalPlan.DailyCost * rentalPlan.PenaltyRate;
                    totalCost -= discount;
                }

                rental.EndDate = returnDate;
                rental.TotalCost = totalCost;

                await _rentalRepository.UpdateAsync(rental);
                await _rentalRepository.SaveChangesAsync();

                var responseDto = await _rentalRepository.GetRentalDetailsAsync(rental.IdRental);

                return new ReturnAPI<RentalResponseDTO>(HttpStatusCode.OK, responseDto);
            }

            catch (Exception)
            {
                _rentalRepository.Rollback();
                throw;
            }

        }

        public async Task<ReturnAPIDataTable<IEnumerable<RentalResponseDTO>>> GetActiveRentalsAsync(FilterPage model)
        {
            ReturnAPIDataTable<IEnumerable<RentalResponseDTO>> ret = new ReturnAPIDataTable<IEnumerable<RentalResponseDTO>>(model);

            var responseDtos = await _rentalRepository.GetAllRentalDetailsAsync(model);

            ret.TotalRecords = responseDtos.total;
            ret.Data = responseDtos.result;
            
            return ret;
        }

    }
}
