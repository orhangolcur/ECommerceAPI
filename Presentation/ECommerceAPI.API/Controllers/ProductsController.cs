using ECommerceAPI.Application.Abstractions.Storage;
using ECommerceAPI.Application.CustomAttributes;
using ECommerceAPI.Application.Enums;
using ECommerceAPI.Application.Features.Commands.ChangeShowcaseImage;
using ECommerceAPI.Application.Features.Commands.Product.CreateProduct;
using ECommerceAPI.Application.Features.Commands.Product.RemoveProduct;
using ECommerceAPI.Application.Features.Commands.Product.UpdateProduct;
using ECommerceAPI.Application.Features.Commands.ProductImageFile.RemoveProductImage;
using ECommerceAPI.Application.Features.Commands.ProductImageFile.UploadProductImage;
using ECommerceAPI.Application.Features.Queries.Product.GetAllProducts;
using ECommerceAPI.Application.Features.Queries.Product.GetByIdProduct;
using ECommerceAPI.Application.Features.Queries.ProductImageFile.GetProductImages;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace ECommerceAPI.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {

        readonly IMediator _mediator; // application'da registration işlemi yapıldıktan sonra burada kullanılabilir.

        public ProductsController(
            IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] GetAllProductsQueryRequest getAllProductsQueryRequest)
        {
            GetAllProductsQueryResponse response = await _mediator.Send(getAllProductsQueryRequest);
            return Ok(response);
        }

        [HttpGet("{Id}")]
        public async Task<IActionResult> Get([FromRoute] GetByIdProductQueryRequest getByIdProductQueryRequest)
        {
            GetByIdProductQueryResponse response = await _mediator.Send(getByIdProductQueryRequest);
            return Ok(response);
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = "Admin")]
        [AuthorizeDefinition(Menu = "Products", ActionType = ActionType.Writing, Definition = "Create Product")]
        public async Task<IActionResult> Post(CreateProductCommandRequest createProductCommandRequest)
        {

            CreateProductCommandResponse response = await _mediator.Send(createProductCommandRequest);
            return StatusCode((int)HttpStatusCode.Created);
        }

        [HttpPut]
        [Authorize(AuthenticationSchemes = "Admin")]
        [AuthorizeDefinition(Menu = "Products", ActionType = ActionType.Updating, Definition = "Update Product")]
        public async Task<IActionResult> Put([FromBody] UpdateProductCommandRequest updateProductCommandRequest)
        {
            UpdateProductCommandResponse response = await _mediator.Send(updateProductCommandRequest);
            return Ok();
        }

        [HttpDelete("{Id}")]
        [Authorize(AuthenticationSchemes = "Admin")]
        [AuthorizeDefinition(Menu = "Products", ActionType = ActionType.Deleting, Definition = "Remove Product")]
        public async Task<IActionResult> Delete([FromRoute] RemoveProductCommandRequest removeProductCommandRequest)
        {
            RemoveProductCommandResponse response = await _mediator.Send(removeProductCommandRequest);
            return Ok();
        }

        [HttpPost("[action]")] // bu şekilde action'ın parametresi route'da belirtilmiyorsa id query string olarak gelir. örn: /api/products/upload?id=1
        [Authorize(AuthenticationSchemes = "Admin")]
        [AuthorizeDefinition(Menu = "Products", ActionType = ActionType.Writing, Definition = "Upload Product Image")]
        public async Task<IActionResult> Upload([FromQuery] UploadProductImageCommandRequest uploadProductImageCommandRequest)
        {
            uploadProductImageCommandRequest.Files = Request.Form.Files;
            UploadProductImageCommandResponse response = await _mediator.Send(uploadProductImageCommandRequest);
            return Ok();
        }

        [HttpGet("[action]/{Id}")] // Bu şekilde action'ın parametresi route'da belirtiliyorsa id route parametresi(route data) olarak gelir. örn: /api/products/getproductimages/1
        [Authorize(AuthenticationSchemes = "Admin")]
        [AuthorizeDefinition(Menu = "Products", ActionType = ActionType.Reading, Definition = "Get Product Images")]
        public async Task<IActionResult> GetProductImages([FromRoute] GetProductImagesQueryRequest getProductImagesQueryRequest)
        {
            List<GetProductImagesQueryResponse> response = await _mediator.Send(getProductImagesQueryRequest);
            return Ok(response);

        }
        //ne geleceği belliyse route data olarak kullanmak daha mantıklı ama gelecek veri opsional veya değişkense ise query string olarak kullanmak daha mantıklı

        [HttpDelete("[action]/{Id}")] // client tarafındaki delete metodu sadece 1 tane id parametresi aldığı için burada product'ın id'sini route'dan alıyoruz image id'sini ise query string olarak alıyoruz.
        [Authorize(AuthenticationSchemes = "Admin")]
        [AuthorizeDefinition(Menu = "Products", ActionType = ActionType.Deleting, Definition = "Remove Product Image")]
        public async Task<IActionResult> DeleteProductImage([FromRoute]RemoveProductImageCommandRequest removeProductImageCommandRequest, [FromQuery] string imageId)
        {
            removeProductImageCommandRequest.ImageId = imageId;
            RemoveProductImageCommandResponse response = await _mediator.Send(removeProductImageCommandRequest);
            return Ok();
        }

        [HttpGet("[action]")]
        [Authorize(AuthenticationSchemes = "Admin")]
        [AuthorizeDefinition(Menu = "Products", ActionType = ActionType.Updating, Definition = "Change Showcase Image")]
        public async Task<IActionResult> ChangeShowCaseImage([FromQuery]ChangeShowcaseImageCommandRequest changeShowcaseImageCommandRequest)
        {
            ChangeShowcaseImageCommandResponse response = await _mediator.Send(changeShowcaseImageCommandRequest);
            return Ok(response);
        }
    }
}