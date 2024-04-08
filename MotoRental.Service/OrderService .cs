using MotoRental.Core.DTO.OrderService;
using MotoRental.Core.Entity;
using MotoRental.Core.Entity.SchemaCore;
using MotoRental.Core.Enum;
using MotoRental.Core.Exceptions;
using MotoRental.Core.ResponseDefault;
using MotoRental.Infrastructure.Repository.Interface;
using MotoRental.Service.Interface;
using System;
using System.Net;
using System.Threading.Tasks;

namespace MotoRental.Service
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IDeliveryPersonRepository _deliveryPersonRepository;
        private readonly IRentalRepository _rentalRepository;
        private readonly INotificationRepository _notificationRepository;
        private readonly INotificationService _notificationService;

        public OrderService(IOrderRepository orderRepository, IDeliveryPersonRepository deliveryPersonRepository, IRentalRepository rentalRepository, INotificationRepository notificationRepository, INotificationService notificationService)
        {
            _orderRepository = orderRepository;
            _deliveryPersonRepository = deliveryPersonRepository;
            _rentalRepository = rentalRepository;
            _notificationRepository = notificationRepository;
            _notificationService = notificationService;
        }

        public async Task<ReturnAPI<OrderResponseDTO>> AddOrderAsync(OrderCreateDTO orderDto)
        {
            try
            {
                var order = new Order
                {
                    RaceValue = orderDto.RaceValue,
                    CreationDate = DateTime.Now,
                    IdOrderStatus = (int)OrderStatusEnum.Disponível
                };

                order = await _orderRepository.AddGetEntityAsync(order);
                await _orderRepository.SaveChangesAsync();


                var eligibleDeliveryPersons = await _deliveryPersonRepository.GetEligibleDeliveryPersonsForOrder();
                await _notificationService.NotifyDeliveryPersonAsync(eligibleDeliveryPersons, order.IdOrder);

                var orderResponse = await _orderRepository.GetOrderDetailsAsync(order.IdOrder);

                return new ReturnAPI<OrderResponseDTO>(HttpStatusCode.Created, orderResponse);
            }
            catch (Exception)
            {
                _orderRepository.Rollback();
                throw;
            }
        }

        public async Task<ReturnAPI> UpdateAcceptOrderAsync(int orderId, int deliveryPersonId)
        {
            try
            {
                var deliveryPerson = await _deliveryPersonRepository.GetByIdAsync(w => w.IdDeliveryPerson == deliveryPersonId);

                if (deliveryPerson == null)
                {
                    throw new NotFoundException($"Delivery person with ID {deliveryPersonId} not found.");
                }

                var order = await _orderRepository.GetByIdAsync(w => w.IdOrder == orderId);
                if (order == null)
                {
                    throw new NotFoundException($"Order with ID {orderId} not found.");
                }

                var isActiveRental = await _rentalRepository.IsAnyRentalActiveForDeliveryPerson(deliveryPersonId);
                if (!isActiveRental)
                {
                    throw new BadRequestException("Delivery person does not have an active rental.");
                }

                var isnotification = await _notificationRepository.GetByIdAsync(w => w.IdDeliveryPerson == deliveryPersonId && w.IdOrder == orderId);

            if (isnotification != null)
                {
                    order.IdOrderStatus = (int)OrderStatusEnum.Aceito;
                    order.IdDeliveryPerson = deliveryPersonId;

                    await _orderRepository.UpdateAsync(order);
                    await _orderRepository.SaveChangesAsync();

                    return new ReturnAPI(HttpStatusCode.OK)
                    {
                        Message = "Order accepted successfully."
                    };
                }
                else
                {
                    throw new BadRequestException("Delivery person was not notified about this order.");
                }

            }
            catch (Exception)
            {
                _orderRepository.Rollback();
                throw;
            }

        }

        public async Task<ReturnAPI> UpdateCompleteOrderAsync(int orderId, int deliveryPersonId)
        {
            try
            {
                var order = await _orderRepository.GetByIdAsync(w => w.IdOrder == orderId);

                if (order == null || order.IdOrder == (int)OrderStatusEnum.Aceito)
                {
                    throw new NotFoundException($"Order with ID {orderId} not found or not in an acceptable state.");
                }

                var isActiveRental = await _rentalRepository.IsAnyRentalActiveForDeliveryPerson(deliveryPersonId);
                if (!isActiveRental)
                {
                    throw new BadRequestException("Delivery person does not have an active rental.");
                }

                order.IdOrder = (int)OrderStatusEnum.Entregue;
                await _orderRepository.UpdateAsync(order);
                await _orderRepository.SaveChangesAsync();

                return new ReturnAPI(HttpStatusCode.OK)
                {
                    Message = "Order completed successfully."
                };

            }
            catch (Exception)
            {
                _orderRepository.Rollback();
                throw;
            }
        }
    }
}
