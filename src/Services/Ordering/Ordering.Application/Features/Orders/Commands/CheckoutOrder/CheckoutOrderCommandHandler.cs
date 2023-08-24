using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Ordering.Application.Models;
using Ordering.Application.ServiceContracts;
using Ordering.Domain.Entities;
using Ordering.Domain.RepositoryContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordering.Application.Features.Orders.Commands.CheckoutOrder
{
    public class CheckoutOrderCommandHandler : IRequestHandler<CheckoutOrderCommand, int>
    {
        private readonly IOrderRepository orderRepository;
        private readonly IMapper mapper;
        private readonly IEmailService emailService;
        private readonly ILogger<CheckoutOrderCommandHandler> logger;

        public CheckoutOrderCommandHandler(IOrderRepository orderRepository, IMapper mapper, IEmailService emailService, ILogger<CheckoutOrderCommandHandler> logger)
        {
            this.orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            this.emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<int> Handle(CheckoutOrderCommand request, CancellationToken cancellationToken)
        {
            var orderRequest = mapper.Map<Order>(request);
            var createdOrder = await orderRepository.AddAsync(orderRequest);

            logger.LogInformation($"Order {createdOrder.Id} is successfully created.");

            await SendMail(createdOrder);

            return createdOrder.Id;
        }

        private async Task SendMail(Order order)
        { // TODO: Use Identity Service to get mail
            var email = new Email()
            {
                To = "oa.mehmetdmrc@gmail.com",
                Body = $"Order was created.",
                Subject = "Order was created."
            };

            try
            {
                await emailService.SendEmail(email);
            }
            catch (Exception e)
            {
                logger.LogError($"Order {order.Id} failed due to an error with the mail service: {e.Message}");
                throw;
            }
        }
    }
}
