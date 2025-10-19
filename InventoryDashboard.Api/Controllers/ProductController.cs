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
    [Consumes("application/json")]
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
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AddProductWebResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(AddProductWebResponse))]
        public async Task<IActionResult> AddProduct([FromBody] AddProductWebRequest request)
        {
            var currentUser = this.GetCurrentUser();
            var serviceRequest = request.ToAddProductRequest(currentUser.UserId);
            var result = await this.productService.AddProductAsync(serviceRequest);
            return this.CreateResponse(result.AsAddProductWebResponse());
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
    }
}
