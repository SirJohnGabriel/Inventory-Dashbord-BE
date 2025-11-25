namespace InventoryDashboard.Api.Controllers
{
    using InventoryDashboard.Api.Common.Extensions;
    using InventoryDashboard.Api.Extensions.Adapters;
    using InventoryDashboard.Api.Extensions.Models.Product;
    using InventoryDashboard.Api.Messages.Product;
    using InventoryDashboard.Infrastructure.Services.Interfaces;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [Produces("application/json")]
    [Route("api/products")]
    [ApiController]
    [Authorize]
    public class ProductController : ControllerBase
    {
        private readonly IProductService productService;
        private readonly ITaxService taxService;

        public ProductController(IProductService productService, ITaxService taxService)
        {
            this.productService = productService;
            this.taxService = taxService;
        }

        [HttpPost]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AddProductWebResponseV2))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(AddProductWebResponseV2))]
        public async Task<IActionResult> AddProduct([FromBody] AddProductWebRequest request, [FromQuery] string currency = null)
        {
            var currentUser = this.GetCurrentUser();
            var serviceRequest = request.ToAddProductRequest(currentUser.UserId);
            var legacyResult = await this.productService.AddProductAsync(serviceRequest, currency);
            var webResponseV2 = legacyResult.ToWebResponseWithTax(this.taxService);

            return this.CreateResponse(webResponseV2);
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetProductsWebResponseV2))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(GetProductsWebResponseV2))]
        public async Task<IActionResult> GetProducts([FromQuery] string currency = null)
        {
            var legacyResult = await this.productService.GetProductsAsync(currency);
            var webResponseV2 = legacyResult.ToWebResponseWithTax(this.taxService);

            return this.CreateResponse(webResponseV2);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetProductWebResponseV2))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(GetProductWebResponseV2))]
        public async Task<IActionResult> GetProductById([FromRoute] Guid id, [FromQuery] string currency = null)
        {
            var legacyResult = await this.productService.GetProductByIdAsync(id, currency);
            var webResponseV2 = legacyResult.ToWebResponseWithTax(this.taxService);

            return this.CreateResponse(webResponseV2);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(DeleteProductWebResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(DeleteProductWebResponse))]
        public async Task<IActionResult> DeleteProductById([FromRoute] Guid id)
        {
            var currentUser = this.GetCurrentUser();
            var result = await this.productService.DeleteProductByIdAsync(id, currentUser.UserId);
            return this.CreateResponse(result.AsDeleteProductWebResponse());
        }

        [HttpPut("{id}")]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UpdateProductWebResponseV2))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(UpdateProductWebResponseV2))]
        public async Task<IActionResult> UpdateProduct([FromRoute] Guid id, [FromBody] UpdateProductWebRequest request, [FromQuery] string currency = null)
        {
            var currentUser = this.GetCurrentUser();
            var serviceRequest = request.ToUpdateProductRequest(id, currentUser.UserId);

            var legacyResult = await this.productService.UpdateProductAsync(serviceRequest, currency);
            var webResponseV2 = legacyResult.ToWebResponseWithTax(this.taxService);

            return this.CreateResponse(webResponseV2);
        }
    }
}
