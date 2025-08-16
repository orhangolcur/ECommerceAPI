using ECommerceAPI.Application.Constants;
using ECommerceAPI.Application.CustomAttributes;
using ECommerceAPI.Application.Enums;
using ECommerceAPI.Application.Features.Commands.Basket.AddItemToBasket;
using ECommerceAPI.Application.Features.Commands.Basket.RemoveBasketItem;
using ECommerceAPI.Application.Features.Commands.Basket.UpdateQuantity;
using ECommerceAPI.Application.Features.Queries.Basket.GetBasketItems;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceAPI.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes ="Admin")]
    public class BasketController : ControllerBase
    {
        readonly IMediator _mediator;
         
        public BasketController(IMediator mediator)
        {
            _mediator = mediator;
        }

         
        [HttpGet]
        [AuthorizeDefinition(Menu = AuthorizeDefinitionContants.Baskets, ActionType = ActionType.Reading, Definition = "Get Basket Items")]
        public async Task<IActionResult> GetBasketItems([FromQuery]GetBasketItemsQueryRequest getBasketItemsQueryRequest)
        {
            List<GetBasketItemsQueryResponse> response = await _mediator.Send(getBasketItemsQueryRequest);
            return Ok(response);
        }

        [HttpPost]
        [AuthorizeDefinition(Menu = AuthorizeDefinitionContants.Baskets, ActionType = ActionType.Writing, Definition = "Add Item To Basket")]
        public async Task<IActionResult> AddItemToBasket(AddItemToBasketCommanRequest addItemToBasketCommanRequest)
        {
            AddItemToBasketCommanResponse response = await _mediator.Send(addItemToBasketCommanRequest);
            return Ok(response);
        }
        [HttpPut]
        [AuthorizeDefinition(Menu = AuthorizeDefinitionContants.Baskets, ActionType = ActionType.Updating, Definition = "Update Quantity")]
        public async Task<IActionResult> UpdateQuantity(UpdateQuantityCommandRequest updateQuantityCommandRequest)
        {
            UpdateQuantityCommandResponse response = await _mediator.Send(updateQuantityCommandRequest);
            return Ok(response);
        }
        [HttpDelete("{BasketItemId}")]
        [AuthorizeDefinition(Menu = AuthorizeDefinitionContants.Baskets, ActionType = ActionType.Deleting, Definition = "Remove Basket Item")]
        public async Task<IActionResult> RemoveBasketItem([FromRoute]RemoveBasketItemCommandRequest removeBasketItemCommandRequest)
        {
            RemoveBasketItemCommandResponse response = await _mediator.Send(removeBasketItemCommandRequest);
            return Ok(response);
        }
    }
}
