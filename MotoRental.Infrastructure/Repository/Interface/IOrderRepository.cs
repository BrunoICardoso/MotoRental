using MotoRental.Core.DTO.OrderService;
using MotoRental.Core.Entity.SchemaCore;
using MotoRental.Infrastructure.Repository.Settings.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotoRental.Infrastructure.Repository.Interface
{
    public interface IOrderRepository : IDomainRepository<Order>
    {
        Task<OrderResponseDTO> GetOrderDetailsAsync(int orderId);
    }
}
