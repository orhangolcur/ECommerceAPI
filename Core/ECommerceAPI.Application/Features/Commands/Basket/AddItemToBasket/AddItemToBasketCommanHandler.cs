using ECommerceAPI.Application.Abstractions.Services;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceAPI.Application.Features.Commands.Basket.AddItemToBasket
{
    public class AddItemToBasketCommanHandler : IRequestHandler<AddItemToBasketCommanRequest, AddItemToBasketCommanResponse>
    {
        readonly IBasketService _basketService;

        public AddItemToBasketCommanHandler(IBasketService basketService)
        {
            _basketService = basketService;
        }

        public async Task<AddItemToBasketCommanResponse> Handle(AddItemToBasketCommanRequest request, CancellationToken cancellationToken)
        {
            await _basketService.AddItemToBasketAsync(new() {
                ProductId = request.ProductId,
                Quantity = request.Quantity
            });

            return new();
        }
    }
}
