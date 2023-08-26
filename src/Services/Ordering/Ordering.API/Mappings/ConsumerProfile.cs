using AutoMapper;
using EventBus.Messages.Events;
using Ordering.Application.Features.Orders.Commands.CheckoutOrder;

namespace Ordering.API.Mappings
{
    public class ConsumerProfile : Profile
    {
        public ConsumerProfile()
        {
            CreateMap<BasketCheckoutEvent, CheckoutOrderCommand>();
        }
    }
}
