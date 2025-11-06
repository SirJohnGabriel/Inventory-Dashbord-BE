namespace InventoryDashboard.Api.Tests.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using InventoryDashboard.Api.Controllers;
    using InventoryDashboard.Api.Messages.Product;
    using InventoryDashboard.Infrastructure.Entities.Products;
    using InventoryDashboard.Infrastructure.Messages;
    using InventoryDashboard.Infrastructure.Messages.Product;
    using InventoryDashboard.Infrastructure.Models.Products;
    using InventoryDashboard.Infrastructure.Services.Interfaces;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    public class ProductControllerTest
    {
        private ProductController target = null!;
        private Mock<IProductService> mockProductService = null!;
        private Mock<ITaxService> mockTaxService = null!;
        private Guid testUserId;

        [TestInitialize]
        public void Setup()
        {
            this.mockProductService = new Mock<IProductService>();
            this.mockTaxService = new Mock<ITaxService>();
            this.testUserId = Guid.NewGuid();

            this.target = new ProductController(
                this.mockProductService.Object,
                this.mockTaxService.Object);

            // Setup user claims for authenticated requests
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, this.testUserId.ToString()),
                new Claim(ClaimTypes.GivenName, "Test"),
                new Claim(ClaimTypes.Surname, "User"),
                new Claim(ClaimTypes.Email, "test@example.com"),
            };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            this.target.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal },
            };
        }

        [TestCleanup]
        public void TearDown()
        {
        }

        #region AddProduct Tests

        [TestMethod]
        public async Task AddProduct_ValidRequest_ReturnsOkStatusCode()
        {
            // Arrange
            var expected = (int)HttpStatusCode.OK;
            var webRequest = new AddProductWebRequest
            {
                Name = "Laptop",
                Description = "Gaming Laptop",
                CategoryId = Guid.NewGuid(),
                Price = 1500m,
                StockQuantity = 10,
                SKU = "LAP001",
            };

            var serviceResponse = new Response<AddProductResponse>
            {
                Data = new AddProductResponse
                {
                    ProductId = Guid.NewGuid().ToString(),
                    Name = "Laptop",
                    Description = "Gaming Laptop",
                    Price = 1500m,
                    StockQuantity = 10,
                    SKU = "LAP001",
                },
            };

            this.mockProductService
                .Setup(s => s.AddProductAsync(It.IsAny<AddProductRequest>()))
                .ReturnsAsync(serviceResponse);

            // Act
            var actual = await this.target.AddProduct(webRequest) as ObjectResult;

            // Assert
            Assert.AreEqual(expected, actual?.StatusCode);
        }

        [TestMethod]
        public async Task AddProduct_ValidRequest_CallsProductServiceWithCorrectUserId()
        {
            // Arrange
            var webRequest = new AddProductWebRequest
            {
                Name = "Laptop",
                CategoryId = Guid.NewGuid(),
                Price = 1500m,
                StockQuantity = 10,
                SKU = "LAP001",
            };

            var serviceResponse = new Response<AddProductResponse>
            {
                Data = new AddProductResponse(),
            };

            this.mockProductService
                .Setup(s => s.AddProductAsync(It.IsAny<AddProductRequest>()))
                .ReturnsAsync(serviceResponse);

            // Act
            await this.target.AddProduct(webRequest);

            // Assert
            this.mockProductService.Verify(
                s => s.AddProductAsync(It.Is<AddProductRequest>(req => req.CreatedBy == this.testUserId)),
                Times.Once);
        }

        [TestMethod]
        public async Task AddProduct_ServiceReturnsError_ReturnsInternalServerErrorStatusCode()
        {
            // Arrange
            var expected = (int)HttpStatusCode.InternalServerError;
            var webRequest = new AddProductWebRequest
            {
                Name = "Laptop",
                CategoryId = Guid.NewGuid(),
                Price = 1500m,
                StockQuantity = 10,
                SKU = "LAP001",
            };

            var serviceResponse = new Response<AddProductResponse>();
            serviceResponse.SetError("PRODUCT_EXISTS", "Product already exists");

            this.mockProductService
                .Setup(s => s.AddProductAsync(It.IsAny<AddProductRequest>()))
                .ReturnsAsync(serviceResponse);

            // Act
            var actual = await this.target.AddProduct(webRequest) as ObjectResult;

            // Assert
            Assert.AreEqual(expected, actual?.StatusCode);
        }

        [TestMethod]
        public async Task AddProduct_ValidRequest_CallsProductServiceOnce()
        {
            // Arrange
            var webRequest = new AddProductWebRequest
            {
                Name = "Laptop",
                CategoryId = Guid.NewGuid(),
                Price = 1500m,
                StockQuantity = 10,
                SKU = "LAP001",
            };

            var serviceResponse = new Response<AddProductResponse>
            {
                Data = new AddProductResponse(),
            };

            this.mockProductService
                .Setup(s => s.AddProductAsync(It.IsAny<AddProductRequest>()))
                .ReturnsAsync(serviceResponse);

            // Act
            await this.target.AddProduct(webRequest);

            // Assert
            this.mockProductService.Verify(s => s.AddProductAsync(It.IsAny<AddProductRequest>()), Times.Once);
        }

        #endregion

        #region GetProducts Tests

        [TestMethod]
        public async Task GetProducts_NoCurrency_ReturnsOkStatusCode()
        {
            // Arrange
            var expected = (int)HttpStatusCode.OK;
            var serviceResponse = new Response<ICollection<GetProductModel>>
            {
                Data = new List<GetProductModel>(),
            };

            this.mockProductService
                .Setup(s => s.GetProductsAsync(null))
                .ReturnsAsync(serviceResponse);

            this.mockTaxService
                .Setup(t => t.GetTaxRateForCategory(It.IsAny<string>()))
                .Returns(0.12m);

            // Act
            var actual = await this.target.GetProducts() as ObjectResult;

            // Assert
            Assert.AreEqual(expected, actual?.StatusCode);
        }

        [TestMethod]
        public async Task GetProducts_WithCurrency_CallsProductServiceWithCurrency()
        {
            // Arrange
            var currency = "USD";
            var serviceResponse = new Response<ICollection<GetProductModel>>
            {
                Data = new List<GetProductModel>(),
            };

            this.mockProductService
                .Setup(s => s.GetProductsAsync(currency))
                .ReturnsAsync(serviceResponse);

            this.mockTaxService
                .Setup(t => t.GetTaxRateForCategory(It.IsAny<string>()))
                .Returns(0.12m);

            // Act
            await this.target.GetProducts(currency);

            // Assert
            this.mockProductService.Verify(s => s.GetProductsAsync(currency), Times.Once);
        }

        [TestMethod]
        public async Task GetProducts_ServiceReturnsProducts_CallsTaxServiceForEachCategory()
        {
            // Arrange
            var categoryId = Guid.NewGuid();
            var category = new Category { Id = categoryId, Name = "Electronics" };
            var products = new List<GetProductModel>
            {
                new GetProductModel { Id = Guid.NewGuid(), Name = "Laptop", Price = 1000m, Category = category },
                new GetProductModel { Id = Guid.NewGuid(), Name = "Mouse", Price = 50m, Category = category },
            };

            var serviceResponse = new Response<ICollection<GetProductModel>>
            {
                Data = products,
            };

            this.mockProductService
                .Setup(s => s.GetProductsAsync(null))
                .ReturnsAsync(serviceResponse);

            this.mockTaxService
                .Setup(t => t.GetTaxRateForCategory(categoryId.ToString()))
                .Returns(0.12m);

            // Act
            await this.target.GetProducts();

            // Assert
            this.mockTaxService.Verify(t => t.GetTaxRateForCategory(categoryId.ToString()), Times.Exactly(2));
        }

        [TestMethod]
        public async Task GetProducts_ServiceReturnsError_ReturnsInternalServerErrorStatusCode()
        {
            // Arrange
            var expected = (int)HttpStatusCode.InternalServerError;
            var serviceResponse = new Response<ICollection<GetProductModel>>();
            serviceResponse.SetError("CURRENCY_NOT_SUPPORTED", "Currency not supported");

            this.mockProductService
                .Setup(s => s.GetProductsAsync(It.IsAny<string>()))
                .ReturnsAsync(serviceResponse);

            // Act
            var actual = await this.target.GetProducts("INVALID") as ObjectResult;

            // Assert
            Assert.AreEqual(expected, actual?.StatusCode);
        }

        [TestMethod]
        public async Task GetProducts_ValidRequest_CallsProductServiceOnce()
        {
            // Arrange
            var serviceResponse = new Response<ICollection<GetProductModel>>
            {
                Data = new List<GetProductModel>(),
            };

            this.mockProductService
                .Setup(s => s.GetProductsAsync(null))
                .ReturnsAsync(serviceResponse);

            this.mockTaxService
                .Setup(t => t.GetTaxRateForCategory(It.IsAny<string>()))
                .Returns(0.12m);

            // Act
            await this.target.GetProducts();

            // Assert
            this.mockProductService.Verify(s => s.GetProductsAsync(null), Times.Once);
        }

        #endregion

        #region GetProductById Tests

        [TestMethod]
        public async Task GetProductById_ValidId_ReturnsOkStatusCode()
        {
            // Arrange
            var expected = (int)HttpStatusCode.OK;
            var productId = Guid.NewGuid();
            var category = new Category { Id = Guid.NewGuid(), Name = "Electronics" };
            var product = new GetProductModel
            {
                Id = productId,
                Name = "Laptop",
                Price = 1000m,
                Category = category,
            };

            var serviceResponse = new Response<GetProductModel>
            {
                Data = product,
            };

            this.mockProductService
                .Setup(s => s.GetProductByIdAsync(productId, null))
                .ReturnsAsync(serviceResponse);

            this.mockTaxService
                .Setup(t => t.GetTaxRateForCategory(It.IsAny<string>()))
                .Returns(0.12m);

            // Act
            var actual = await this.target.GetProductById(productId) as ObjectResult;

            // Assert
            Assert.AreEqual(expected, actual?.StatusCode);
        }

        [TestMethod]
        public async Task GetProductById_WithCurrency_CallsProductServiceWithCurrency()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var currency = "USD";
            var category = new Category { Id = Guid.NewGuid(), Name = "Electronics" };
            var product = new GetProductModel
            {
                Id = productId,
                Name = "Laptop",
                Price = 1000m,
                Category = category,
            };

            var serviceResponse = new Response<GetProductModel>
            {
                Data = product,
            };

            this.mockProductService
                .Setup(s => s.GetProductByIdAsync(productId, currency))
                .ReturnsAsync(serviceResponse);

            this.mockTaxService
                .Setup(t => t.GetTaxRateForCategory(It.IsAny<string>()))
                .Returns(0.12m);

            // Act
            await this.target.GetProductById(productId, currency);

            // Assert
            this.mockProductService.Verify(s => s.GetProductByIdAsync(productId, currency), Times.Once);
        }

        [TestMethod]
        public async Task GetProductById_ProductNotFound_ReturnsInternalServerErrorStatusCode()
        {
            // Arrange
            var expected = (int)HttpStatusCode.InternalServerError;
            var productId = Guid.NewGuid();
            var serviceResponse = new Response<GetProductModel>();
            serviceResponse.SetError("PRODUCT_NOT_FOUND", "Product not found");

            this.mockProductService
                .Setup(s => s.GetProductByIdAsync(productId, null))
                .ReturnsAsync(serviceResponse);

            // Act
            var actual = await this.target.GetProductById(productId) as ObjectResult;

            // Assert
            Assert.AreEqual(expected, actual?.StatusCode);
        }

        [TestMethod]
        public async Task GetProductById_ValidProduct_CallsTaxServiceWithCategoryId()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var categoryId = Guid.NewGuid();
            var category = new Category { Id = categoryId, Name = "Electronics" };
            var product = new GetProductModel
            {
                Id = productId,
                Name = "Laptop",
                Price = 1000m,
                Category = category,
            };

            var serviceResponse = new Response<GetProductModel>
            {
                Data = product,
            };

            this.mockProductService
                .Setup(s => s.GetProductByIdAsync(productId, null))
                .ReturnsAsync(serviceResponse);

            this.mockTaxService
                .Setup(t => t.GetTaxRateForCategory(categoryId.ToString()))
                .Returns(0.12m);

            // Act
            await this.target.GetProductById(productId);

            // Assert
            this.mockTaxService.Verify(t => t.GetTaxRateForCategory(categoryId.ToString()), Times.Once);
        }

        [TestMethod]
        public async Task GetProductById_ValidRequest_CallsProductServiceOnce()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var category = new Category { Id = Guid.NewGuid(), Name = "Electronics" };
            var product = new GetProductModel
            {
                Id = productId,
                Name = "Laptop",
                Price = 1000m,
                Category = category,
            };

            var serviceResponse = new Response<GetProductModel>
            {
                Data = product,
            };

            this.mockProductService
                .Setup(s => s.GetProductByIdAsync(productId, null))
                .ReturnsAsync(serviceResponse);

            this.mockTaxService
                .Setup(t => t.GetTaxRateForCategory(It.IsAny<string>()))
                .Returns(0.12m);

            // Act
            await this.target.GetProductById(productId);

            // Assert
            this.mockProductService.Verify(s => s.GetProductByIdAsync(productId, null), Times.Once);
        }

        #endregion

        #region DeleteProductById Tests

        [TestMethod]
        public async Task DeleteProductById_ValidId_ReturnsOkStatusCode()
        {
            // Arrange
            var expected = (int)HttpStatusCode.OK;
            var productId = Guid.NewGuid();
            var serviceResponse = new Response();

            this.mockProductService
                .Setup(s => s.DeleteProductByIdAsync(productId, this.testUserId))
                .ReturnsAsync(serviceResponse);

            // Act
            var actual = await this.target.DeleteProductById(productId) as ObjectResult;

            // Assert
            Assert.AreEqual(expected, actual?.StatusCode);
        }

        [TestMethod]
        public async Task DeleteProductById_ValidId_CallsProductServiceWithCorrectUserId()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var serviceResponse = new Response();

            this.mockProductService
                .Setup(s => s.DeleteProductByIdAsync(productId, this.testUserId))
                .ReturnsAsync(serviceResponse);

            // Act
            await this.target.DeleteProductById(productId);

            // Assert
            this.mockProductService.Verify(
                s => s.DeleteProductByIdAsync(productId, this.testUserId),
                Times.Once);
        }

        [TestMethod]
        public async Task DeleteProductById_ProductNotFound_ReturnsInternalServerErrorStatusCode()
        {
            // Arrange
            var expected = (int)HttpStatusCode.InternalServerError;
            var productId = Guid.NewGuid();
            var serviceResponse = new Response();
            serviceResponse.SetError("PRODUCT_NOT_FOUND", "Product not found");

            this.mockProductService
                .Setup(s => s.DeleteProductByIdAsync(productId, this.testUserId))
                .ReturnsAsync(serviceResponse);

            // Act
            var actual = await this.target.DeleteProductById(productId) as ObjectResult;

            // Assert
            Assert.AreEqual(expected, actual?.StatusCode);
        }

        [TestMethod]
        public async Task DeleteProductById_ValidRequest_CallsProductServiceOnce()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var serviceResponse = new Response();

            this.mockProductService
                .Setup(s => s.DeleteProductByIdAsync(productId, this.testUserId))
                .ReturnsAsync(serviceResponse);

            // Act
            await this.target.DeleteProductById(productId);

            // Assert
            this.mockProductService.Verify(
                s => s.DeleteProductByIdAsync(productId, this.testUserId),
                Times.Once);
        }

        #endregion

        #region UpdateProduct Tests

        [TestMethod]
        public async Task UpdateProduct_ValidRequest_ReturnsOkStatusCode()
        {
            // Arrange
            var expected = (int)HttpStatusCode.OK;
            var productId = Guid.NewGuid();
            var webRequest = new UpdateProductWebRequest
            {
                Name = "Updated Laptop",
                Price = 1600m,
            };

            var serviceResponse = new Response<UpdateProductResponse>
            {
                Data = new UpdateProductResponse
                {
                    ProductId = productId.ToString(),
                    Name = "Updated Laptop",
                    Price = 1600m,
                },
            };

            this.mockProductService
                .Setup(s => s.UpdateProductAsync(It.IsAny<UpdateProductRequest>()))
                .ReturnsAsync(serviceResponse);

            // Act
            var actual = await this.target.UpdateProduct(productId, webRequest) as ObjectResult;

            // Assert
            Assert.AreEqual(expected, actual?.StatusCode);
        }

        [TestMethod]
        public async Task UpdateProduct_ValidRequest_CallsProductServiceWithCorrectProductId()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var webRequest = new UpdateProductWebRequest
            {
                Name = "Updated Laptop",
            };

            var serviceResponse = new Response<UpdateProductResponse>
            {
                Data = new UpdateProductResponse(),
            };

            this.mockProductService
                .Setup(s => s.UpdateProductAsync(It.IsAny<UpdateProductRequest>()))
                .ReturnsAsync(serviceResponse);

            // Act
            await this.target.UpdateProduct(productId, webRequest);

            // Assert
            this.mockProductService.Verify(
                s => s.UpdateProductAsync(It.Is<UpdateProductRequest>(req => req.Id == productId)),
                Times.Once);
        }

        [TestMethod]
        public async Task UpdateProduct_ValidRequest_CallsProductServiceWithCorrectUserId()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var webRequest = new UpdateProductWebRequest
            {
                Name = "Updated Laptop",
            };

            var serviceResponse = new Response<UpdateProductResponse>
            {
                Data = new UpdateProductResponse(),
            };

            this.mockProductService
                .Setup(s => s.UpdateProductAsync(It.IsAny<UpdateProductRequest>()))
                .ReturnsAsync(serviceResponse);

            // Act
            await this.target.UpdateProduct(productId, webRequest);

            // Assert
            this.mockProductService.Verify(
                s => s.UpdateProductAsync(It.Is<UpdateProductRequest>(req => req.UpdatedBy == this.testUserId)),
                Times.Once);
        }

        [TestMethod]
        public async Task UpdateProduct_ProductNotFound_ReturnsInternalServerErrorStatusCode()
        {
            // Arrange
            var expected = (int)HttpStatusCode.InternalServerError;
            var productId = Guid.NewGuid();
            var webRequest = new UpdateProductWebRequest
            {
                Name = "Updated Laptop",
            };

            var serviceResponse = new Response<UpdateProductResponse>();
            serviceResponse.SetError("PRODUCT_NOT_FOUND", "Product not found");

            this.mockProductService
                .Setup(s => s.UpdateProductAsync(It.IsAny<UpdateProductRequest>()))
                .ReturnsAsync(serviceResponse);

            // Act
            var actual = await this.target.UpdateProduct(productId, webRequest) as ObjectResult;

            // Assert
            Assert.AreEqual(expected, actual?.StatusCode);
        }

        [TestMethod]
        public async Task UpdateProduct_ValidRequest_CallsProductServiceOnce()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var webRequest = new UpdateProductWebRequest
            {
                Name = "Updated Laptop",
            };

            var serviceResponse = new Response<UpdateProductResponse>
            {
                Data = new UpdateProductResponse(),
            };

            this.mockProductService
                .Setup(s => s.UpdateProductAsync(It.IsAny<UpdateProductRequest>()))
                .ReturnsAsync(serviceResponse);

            // Act
            await this.target.UpdateProduct(productId, webRequest);

            // Assert
            this.mockProductService.Verify(s => s.UpdateProductAsync(It.IsAny<UpdateProductRequest>()), Times.Once);
        }

        #endregion
    }
}