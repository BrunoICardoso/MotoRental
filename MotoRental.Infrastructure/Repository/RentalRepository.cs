using Dapper;
using MotoRental.Core.DTO.RentalService;
using MotoRental.Core.Entity.SchemaCore;
using MotoRental.Core.ResponseDefault;
using MotoRental.Infrastructure.Repository.Interface;
using MotoRental.Infrastructure.Repository.Settings;
using MotoRental.Infrastructure.Repository.Settings.Interface;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotoRental.Infrastructure.Repository
{
    public class RentalRepository : DomainRepository<Rental>, IRentalRepository
    {
        public RentalRepository(IDbConnection databaseConnection) : base(databaseConnection)
        {
        }

        public RentalRepository(IDatabaseFactory databaseOptions) : base(databaseOptions)
        {
        }

        public async Task<bool> IsAnyRentalActiveForMoto(int motoId)
        {
            var currentDate = DateTime.Now;

            var query = $@"
            SELECT EXISTS (
                SELECT 1
                FROM core.""Rental""
                WHERE ""IdMoto"" = @{nameof(motoId)}
                AND (""EndDate"" IS NULL OR ""EndDate"" > @{nameof(currentDate)})
           ) AS ""IsRented""";

            var result = await dbConn.ExecuteScalarAsync<bool>(query, new { motoId, currentDate });
            return result;
        }

        public async Task<(IEnumerable<RentalResponseDTO> result, int total)> GetAllRentalDetailsAsync(FilterPage model)
        {
            string select = $@"SELECT
            r.""IdRental"" AS ""RentalId"",
            r.""StartDate"",
            r.""EndDate"",
            r.""ExpectedEndDate"",
            r.""DailyCost"",
            r.""TotalCost"",
            dp.""IdDeliveryPerson"",
            dp.""Name"",
            dp.""CNPJ"",
            dp.""BirthDate"",
            dp.""CNHNumber"",
            dp.""IdCNHType"" AS ""CNHType"",
            dp.""CNHImagePath"",
            dp.""Active"",
            m.""IdMoto"",
            m.""Year"",
            m.""Model"",
            m.""LicensePlate"",
            m.""Active""";
            string from = $@"FROM core.""Rental"" r
            INNER JOIN core.""DeliveryPerson"" dp ON r.""IdDeliveryPerson"" = dp.""IdDeliveryPerson""
            INNER JOIN core.""Moto"" m ON r.""IdMoto"" = m.""IdMoto""
            WHERE r.""EndDate"" IS NULL";

            string query = $@"{select} {from} 
             ORDER BY r.""IdRental"" 
             LIMIT @{nameof(model.Size)}  OFFSET (@{nameof(model.Skip)} - 1) * @{nameof(model.Size)};
            
            SELECT COUNT(1) 
            {from};";


            using var multi = await dbConn.QueryMultipleAsync(sql: query, param: new {model.Skip, model.Size });



            var rentalDetails = multi.Read<RentalResponseDTO, RentalDeliveryPersonDTO, RentalMotoDTO, RentalResponseDTO>(
                                  (rental, deliveryPerson, moto) =>
                                  {
                                      rental.RentalDeliveryPersonMoto = new RentalDeliveryPersonMotoDTO { DeliveryPerson = deliveryPerson, Moto = moto };
                                      return rental;
                                  },
                                  splitOn: "IdDeliveryPerson,IdMoto"
                                  );

            int total = (await multi.ReadFirstOrDefaultAsync<int>());

            return (rentalDetails, total);
        }


        public async Task<RentalResponseDTO> GetRentalDetailsAsync(int rentalId)
        {
            var query = $@"SELECT
            r.""IdRental"" AS ""RentalId"",
            r.""StartDate"",
            r.""EndDate"",
            r.""ExpectedEndDate"",
            r.""DailyCost"",
            r.""TotalCost"",
            dp.""IdDeliveryPerson"",
            dp.""Name"",
            dp.""CNPJ"",
            dp.""BirthDate"",
            dp.""CNHNumber"",
            dp.""IdCNHType"" AS ""CNHType"",
            dp.""CNHImagePath"",
            dp.""Active"",
            m.""IdMoto"",
            m.""Year"",
            m.""Model"",
            m.""LicensePlate"",
            m.""Active""
            FROM core.""Rental"" r
            INNER JOIN core.""DeliveryPerson"" dp ON r.""IdDeliveryPerson"" = dp.""IdDeliveryPerson""
            INNER JOIN core.""Moto"" m ON r.""IdMoto"" = m.""IdMoto""
            WHERE r.""IdRental"" = @{nameof(rentalId)}";


            var rentalDetails = await dbConn.QueryAsync<RentalResponseDTO, RentalDeliveryPersonDTO, RentalMotoDTO, RentalResponseDTO>(
                query,
                (rental, deliveryPerson, moto) =>
                {
                    rental.RentalDeliveryPersonMoto = new RentalDeliveryPersonMotoDTO { DeliveryPerson = deliveryPerson, Moto = moto };
                    return rental;
                },
                splitOn: "IdDeliveryPerson,IdMoto",
                param: new { RentalId = rentalId });

            return rentalDetails.FirstOrDefault();
        }

        public async Task<bool> IsAnyRentalActiveForDeliveryPerson(int deliveryPersonId) 
        {
            string query = $@"SELECT EXISTS (
                            SELECT 1
                            FROM ""core"".""Rental"" r
                            JOIN ""core"".""DeliveryPerson"" dp ON r.""IdDeliveryPerson"" = dp.""IdDeliveryPerson""
                            WHERE dp.""IdDeliveryPerson"" = @{nameof(deliveryPersonId)}
                            AND r.""EndDate"" IS NULL);";

            return await dbConn.ExecuteScalarAsync<bool>(query, new {deliveryPersonId });
        }
    }
}
