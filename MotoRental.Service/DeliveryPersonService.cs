using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using MotoRental.Core.Appsettings;
using MotoRental.Core.DTO.DeliveryPersonService;
using MotoRental.Core.Entity;
using MotoRental.Core.Entity.SchemaCore;
using MotoRental.Core.Enum;
using MotoRental.Core.Exceptions;
using MotoRental.Core.ResponseDefault;
using MotoRental.Infrastructure.Repository.Interface;
using MotoRental.Service.Interface;
using System.Net;

public class DeliveryPersonService : IDeliveryPersonService
{
    private readonly IDeliveryPersonRepository _deliveryPersonRepository;
    private readonly FileStorageSettings _fileStorageSettings;

    public DeliveryPersonService(IDeliveryPersonRepository deliveryPersonRepository, IOptions<FileStorageSettings> fileStorageSettings)
    {
        _deliveryPersonRepository = deliveryPersonRepository;
        _fileStorageSettings = fileStorageSettings.Value;
    }

    public async Task<ReturnAPI<DeliveryPersonResponseDTO>> AddDeliveryPersonAsync(int userid, DeliveryPersonCreateDTO deliveryPersonDto)
    {
        ReturnAPI<DeliveryPersonResponseDTO> ret = new ReturnAPI<DeliveryPersonResponseDTO>();

        try
        {
            var existingDeliveryPerson = await _deliveryPersonRepository.GetByIdAsync(w => w.CNPJ == deliveryPersonDto.CNPJ);
            if (existingDeliveryPerson != null)
            {
                throw new BadRequestException("CNPJ already in use.");
            }

            var deliveryPerson = new DeliveryPerson
            {
                BirthDate = deliveryPersonDto.DateOfBirth,
                CNHNumber = deliveryPersonDto.CNHNumber,
                IdCNHType = (int)deliveryPersonDto.CNHType,
                CNPJ = deliveryPersonDto.CNPJ,
                Name = deliveryPersonDto.Name,
                Active = true,
                IdUser = userid
            };

            var IdDeliveryPerson = await _deliveryPersonRepository.AddGetIdentityAsync<int>(deliveryPerson);

            var responseDto = new DeliveryPersonResponseDTO
            {
                Id = IdDeliveryPerson,
                DateOfBirth = deliveryPersonDto.DateOfBirth,
                CNHNumber = deliveryPersonDto.CNHNumber,
                CNHType = deliveryPersonDto.CNHType,
                CNPJ = deliveryPersonDto.CNPJ,
                Name = deliveryPersonDto.Name
            };

            await _deliveryPersonRepository.SaveChangesAsync();
        }
        catch (Exception)
        {
            _deliveryPersonRepository.Rollback();
            throw;
        }

        return ret;
    }

    public async Task<ReturnAPI> UpdateCnhAsync(int deliveryPersonId, UpdateCNHFileDTO model)
    {
        try
        {
            var deliveryPerson = await _deliveryPersonRepository.GetByIdAsync(w => w.IdDeliveryPerson == deliveryPersonId);
            if (deliveryPerson == null)
            {
                throw new NotFoundException("Delivery person not found.");
            }

            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(model.File.FileName);
            var fullPath = Path.Combine(_fileStorageSettings.BasePath, fileName);


            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await model.File.CopyToAsync(stream);
            }

            deliveryPerson.CNHImagePath = fullPath;
            await _deliveryPersonRepository.UpdateAsync(deliveryPerson);

            await _deliveryPersonRepository.SaveChangesAsync();

            return new ReturnAPI(HttpStatusCode.OK)
            {
                Message = "CNH updated successfully."
            };
        }
        catch (Exception)
        {
            _deliveryPersonRepository.Rollback();
            throw;
        }
        
    }

    public async Task<ReturnAPI> UpdateDeliveryPersonAsync(int deliveryPersonId, DeliveryPersonUpdateDTO deliveryPersonDto)
    {
        try
        {
            var deliveryPerson = await _deliveryPersonRepository.GetByIdAsync(w => w.IdDeliveryPerson == deliveryPersonId);
            if (deliveryPerson == null)
            {
                throw new NotFoundException("Delivery person not found.");
            }

            if (!string.IsNullOrWhiteSpace(deliveryPersonDto.Name))
            {
                deliveryPerson.Name = deliveryPersonDto.Name;
            }
            if (deliveryPersonDto.DateOfBirth.HasValue)
            {
                deliveryPerson.BirthDate = deliveryPersonDto.DateOfBirth.Value;
            }
            if (!string.IsNullOrWhiteSpace(deliveryPersonDto.CNHNumber))
            {
                deliveryPerson.CNHNumber = deliveryPersonDto.CNHNumber;
            }

            if (deliveryPersonDto.CNHType.HasValue)
            {
                deliveryPerson.IdCNHType = (int)deliveryPersonDto.CNHType.Value;
            }

            await _deliveryPersonRepository.UpdateAsync(deliveryPerson);
            await _deliveryPersonRepository.SaveChangesAsync();

            return new ReturnAPI(HttpStatusCode.OK)
            {
                Message = "Delivery person updated successfully."
            };
        }
        catch (Exception)
        {
            _deliveryPersonRepository.Rollback();
            throw;
        }

    }


    public async Task<ReturnAPI<IEnumerable<DeliveryPersonResponseDTO>>> GetAllDeliveryPersonsAsync()
    {
        var deliveryPersons = await _deliveryPersonRepository.GetAllAsync(o => o.Name);
        var responseDtos = deliveryPersons.Select(dp => new DeliveryPersonResponseDTO
        {
            Id = dp.IdDeliveryPerson,
            Name = dp.Name,
            CNPJ = dp.CNPJ,
            CNHImagePath = dp.CNHImagePath,
            CNHNumber = dp.CNHNumber,
            CNHType = (CNHTypeEnum)dp.IdCNHType,
            DateOfBirth = dp.BirthDate,
            Active = dp.Active
        });

        return new ReturnAPI<IEnumerable<DeliveryPersonResponseDTO>>(HttpStatusCode.OK, responseDtos);
    }

    public async Task<ReturnAPI<DeliveryPersonResponseDTO>> GetDeliveryPersonByIdAsync(int id)
    {
        var deliveryPerson = await _deliveryPersonRepository.GetByIdAsync(w => w.IdDeliveryPerson == id);
        if (deliveryPerson == null)
        {
            throw new NotFoundException("Delivery person not found.");
        }

        var responseDto = new DeliveryPersonResponseDTO
        {
            Id = deliveryPerson.IdDeliveryPerson,
            Name = deliveryPerson.Name,
            CNPJ = deliveryPerson.CNPJ,
            CNHImagePath = deliveryPerson.CNHImagePath,
            CNHNumber = deliveryPerson.CNHNumber,
            CNHType = (CNHTypeEnum)deliveryPerson.IdCNHType,
            DateOfBirth = deliveryPerson.BirthDate,
            Active = deliveryPerson.Active
        };

        return new ReturnAPI<DeliveryPersonResponseDTO>(HttpStatusCode.OK, responseDto);
    }

    public async Task<ReturnAPI> DeleteDeliveryPersonAsync(int id)
    {
        try
        {
            var deliveryPerson = await _deliveryPersonRepository.GetByIdAsync(w => w.IdDeliveryPerson == id);
            if (deliveryPerson == null)
            {
                throw new NotFoundException("Delivery person not found.");
            }

            deliveryPerson.Active = false;
            await _deliveryPersonRepository.UpdateAsync(deliveryPerson);

            await _deliveryPersonRepository.SaveChangesAsync();
            return new ReturnAPI(HttpStatusCode.OK)
            {
                Message = "Delivery person deactivated successfully."
            };
        }
        catch (Exception)
        {
            _deliveryPersonRepository.Rollback();
            throw;
        }

    }

}
