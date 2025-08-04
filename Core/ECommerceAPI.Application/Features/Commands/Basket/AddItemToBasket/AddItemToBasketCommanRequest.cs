using MediatR;

namespace ECommerceAPI.Application.Features.Commands.Basket.AddItemToBasket
{
    public class AddItemToBasketCommanRequest : IRequest<AddItemToBasketCommanResponse>
    {
        public string ProductId { get; set; }
        public int Quantity { get; set; }
    }
}