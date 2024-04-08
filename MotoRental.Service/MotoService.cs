using Microsoft.Extensions.Logging;
using MotoRental.Core.DTO.MotoService;
using MotoRental.Core.Entity;
using MotoRental.Core.Entity.SchemaCore;
using MotoRental.Core.Exceptions;
using MotoRental.Core.ResponseDefault;
using MotoRental.Infrastructure.Repository.Interface;
using MotoRental.Service.Interface;
using System.Net;

public class MotoService : IMotoService
{
    private readonly IMotoRepository _motoRepository;
    private readonly IRentalRepository _rentalRepository;
    private readonly ILogger<MotoService> _logger;

    public MotoService(IMotoRepository motoRepository, IRentalRepository rentalRepository, ILogger<MotoService> logger)
    {
        _motoRepository = motoRepository;
        _rentalRepository = rentalRepository;
        _logger = logger;
    }

    public async Task<ReturnAPI<MotoResponseDTO>> AddMotoAsync(MotoCreateDTO motoDto)
    {
        try
        {
            var existingMoto = await _motoRepository.GetByIdAsync(w => w.LicensePlate == motoDto.LicensePlate);

            if (existingMoto != null)
            {
                throw new BadRequestException($"The motorcycle with this plate {motoDto.LicensePlate} already exists.");
            }

            var motoentity = await _motoRepository.AddGetEntityAsync(new Moto
            {
                Year = motoDto.Year,
                Model = motoDto.Model,
                LicensePlate = motoDto.LicensePlate,
                Active = true
            });

            var responseDto = new MotoResponseDTO
            {
                Id = motoentity.IdMoto,
                Year = motoentity.Year,
                Model = motoentity.Model,
                LicensePlate = motoentity.LicensePlate
            };
            await _motoRepository.SaveChangesAsync();
            _logger.LogInformation($"New motorcycle added with License Plate: {motoDto.LicensePlate}.");
            return new ReturnAPI<MotoResponseDTO>(HttpStatusCode.Created, responseDto);
        }
        catch (Exception)
        {
            _motoRepository.Rollback();
            throw;
        }

    }

   
    public async Task<ReturnAPI<IEnumerable<MotoResponseDTO>>> GetAllMotosAsync(string licensePlate = null)
    {
        var motos = !string.IsNullOrEmpty(licensePlate) ? await _motoRepository.GetAllAsync( w => w.LicensePlate == licensePlate, o => o.Year) 
                                                        : await _motoRepository.GetAllAsync(o => o.Year);
        
        var motoDtos = motos.Select(m => new MotoResponseDTO
        {
            Id = m.IdMoto,
            Year = m.Year,
            Model = m.Model,
            LicensePlate = m.LicensePlate,
            Active = m.Active
        });

        return new ReturnAPI<IEnumerable<MotoResponseDTO>>(HttpStatusCode.OK, motoDtos);
    }

    public async Task<ReturnAPI<MotoResponseDTO>> GetMotoByIdAsync(int id)
    {
        var moto = await _motoRepository.GetByIdAsync(w=> w.IdMoto == id && w.Active == true);
        if (moto == null)
        {
            throw new NotFoundException($"Moto with ID {id} not found.");
        }

        var responseDto = new MotoResponseDTO
        {
            Id = moto.IdMoto,
            Year = moto.Year,
            Model = moto.Model,
            LicensePlate = moto.LicensePlate
        };

        return new ReturnAPI<MotoResponseDTO>(HttpStatusCode.OK, responseDto);
    }



    public async Task<ReturnAPI> UpdateMotoAsync(int id, MotoUpdateDTO motoDto)
    {
        try
        {
            var motoToUpdate = await _motoRepository.GetByIdAsync(w => w.IdMoto == id);
            if (motoToUpdate == null)
            {
                throw new NotFoundException($"Moto with ID {id} not found.");
            }

            var existingMoto = await _motoRepository.GetByIdAsync(w => w.LicensePlate == motoDto.LicensePlate);
            if (existingMoto != null && existingMoto.IdMoto != id)
            {
                throw new BadRequestException($"Another motorcycle with plate {motoDto.LicensePlate} already exists.");
            }

            if (!string.IsNullOrWhiteSpace(motoDto.LicensePlate))
            {
                motoToUpdate.LicensePlate = motoDto.LicensePlate;
            }

            if (!string.IsNullOrWhiteSpace(motoDto.Model))
            {
                motoToUpdate.Model = motoDto.Model;
            }
            
            if (motoDto.Year.HasValue)
            {
                motoToUpdate.Year = motoDto.Year.Value;
            }

            await _motoRepository.UpdateAsync(motoToUpdate);
            await _motoRepository.SaveChangesAsync();
            return new ReturnAPI(HttpStatusCode.OK)
            {
                Message = $"Moto with ID {id} updated successfully."
            };
        }
        catch (Exception)
        {
            _motoRepository.Rollback();
            throw;
        }
        
    }

    public async Task<ReturnAPI> DeleteMotoAsync(int id)
    {
        try
        {
            var motoToDelete = await _motoRepository.GetByIdAsync(w => w.IdMoto == id);
            if (motoToDelete == null)
            {
                throw new NotFoundException($"Moto with ID {id} not found.");
            }

            var isRented = await _rentalRepository.IsAnyRentalActiveForMoto(id);
            if (isRented)
            {
                throw new BadRequestException($"Moto with ID {id} cannot be deleted because it is currently rented.");
            }

            motoToDelete.Active = false;
            await _motoRepository.UpdateAsync(motoToDelete);
            await _motoRepository.SaveChangesAsync();
            return new ReturnAPI(HttpStatusCode.OK)
            {
                Message = $"Moto with ID {id} deleted successfully."
            };
        }
        catch (Exception)
        {
            _motoRepository.Rollback();
            throw;
        }
        

    }

}
