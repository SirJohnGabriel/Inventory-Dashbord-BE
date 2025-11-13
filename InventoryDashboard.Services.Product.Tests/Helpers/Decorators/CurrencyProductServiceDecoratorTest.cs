namespace InventoryDashboard.Services.Product.Tests.Helpers.Decorators;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InventoryDashboard.Infrastructure.Messages;
using InventoryDashboard.Infrastructure.Messages.Product;
using InventoryDashboard.Infrastructure.Models.Products;
using InventoryDashboard.Infrastructure.Services.Interfaces;
using InventoryDashboard.Services.Product.Helpers.Decorators;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

[TestClass]
public class CurrencyProductServiceDecoratorTest
{
    private Mock<IProductService> mockInnerService = null!;
    private Mock<ICurrencyService> mockCurrencyService = null!;
    private CurrencyProductServiceDecorator decorator = null!;

    [TestInitialize]
    public void Setup()
    {
        this.mockInnerService = new Mock<IProductService>();
        this.mockCurrencyService = new Mock<ICurrencyService>();
        this.decorator = new CurrencyProductServiceDecorator(
            this.mockInnerService.Object,
            this.mockCurrencyService.Object);
    }

    #region AddProductAsync Tests

    [TestMethod]
    public async Task AddProductAsync_ValidRequest_DelegatesToInnerService()
    {
        // Arrange
        var request = new AddProductRequest
        {
            Name = "Laptop",
            Description = "Gaming Laptop",
            CategoryId = Guid.NewGuid(),
            Price = 1500.00m,
            StockQuantity = 10,
            SKU = "LAP001",
            CreatedBy = Guid.NewGuid(),
            UpdatedBy = Guid.NewGuid(),
        };

        var expectedResponse = new Response<AddProductResponse>
        {
            Data = new AddProductResponse
            {
                ProductId = Guid.NewGuid().ToString(),
                Name = "Laptop",
                Description = "Gaming Laptop",
                Price = 1500.00m,
                StockQuantity = 10,
                SKU = "LAP001",
            },
        };

        this.mockInnerService
            .Setup(s => s.AddProductAsync(request))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await this.decorator.AddProductAsync(request);

        // Assert
        Assert.AreEqual(expectedResponse, result);
    }

    [TestMethod]
    public async Task AddProductAsync_ValidRequest_CallsInnerServiceOnce()
    {
        // Arrange
        var request = new AddProductRequest
        {
            Name = "Laptop",
            CategoryId = Guid.NewGuid(),
            Price = 1500.00m,
            StockQuantity = 10,
            SKU = "LAP001",
            CreatedBy = Guid.NewGuid(),
            UpdatedBy = Guid.NewGuid(),
        };

        this.mockInnerService
            .Setup(s => s.AddProductAsync(It.IsAny<AddProductRequest>()))
            .ReturnsAsync(new Response<AddProductResponse>());

        // Act
        await this.decorator.AddProductAsync(request);

        // Assert
        this.mockInnerService.Verify(s => s.AddProductAsync(request), Times.Once);
    }

    #endregion

    #region GetProductsAsync Tests

    [TestMethod]
    public async Task GetProductsAsync_NoCurrencySpecified_DelegatesToInnerServiceWithoutConversion()
    {
        // Arrange
        var products = new List<GetProductModel>
        {
            new GetProductModel
            {
                Id = Guid.NewGuid(),
                Name = "Laptop",
                Price = 50000m,
                SKU = "LAP001",
            },
        };

        var expectedResponse = new Response<ICollection<GetProductModel>> { Data = products };
        this.mockInnerService
            .Setup(s => s.GetProductsAsync(null))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await this.decorator.GetProductsAsync(null);

        // Assert
        Assert.AreEqual(expectedResponse, result);
    }

    [TestMethod]
    public async Task GetProductsAsync_WithTargetCurrency_AppliesCurrencyConversion()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var products = new List<GetProductModel>
        {
            new GetProductModel
            {
                Id = productId,
                Name = "Laptop",
                Price = 50000m,
                SKU = "LAP001",
            },
        };

        var innerResponse = new Response<ICollection<GetProductModel>> { Data = products };
        this.mockInnerService
            .Setup(s => s.GetProductsAsync("USD"))
            .ReturnsAsync(innerResponse);

        this.mockCurrencyService
            .Setup(c => c.Convert(50000m, "PHP", "USD"))
            .Returns(1000m);

        // Act
        var result = await this.decorator.GetProductsAsync("USD");

        // Assert
        Assert.AreEqual(1000m, result.Data.First().ConvertedPrice);
    }

    [TestMethod]
    public async Task GetProductsAsync_WithTargetCurrency_SetsCurrencyCode()
    {
        // Arrange
        var products = new List<GetProductModel>
        {
            new GetProductModel
            {
                Id = Guid.NewGuid(),
                Name = "Laptop",
                Price = 50000m,
                SKU = "LAP001",
            },
        };

        var innerResponse = new Response<ICollection<GetProductModel>> { Data = products };
        this.mockInnerService
            .Setup(s => s.GetProductsAsync("USD"))
            .ReturnsAsync(innerResponse);

        this.mockCurrencyService
            .Setup(c => c.Convert(It.IsAny<decimal>(), It.IsAny<string>(), It.IsAny<string>()))
            .Returns(1000m);

        // Act
        var result = await this.decorator.GetProductsAsync("USD");

        // Assert
        Assert.AreEqual("USD", result.Data.First().CurrencyCode);
    }

    [TestMethod]
    public async Task GetProductsAsync_WithLowercaseCurrency_ConvertsToUppercase()
    {
        // Arrange
        var products = new List<GetProductModel>
        {
            new GetProductModel
            {
                Id = Guid.NewGuid(),
                Name = "Laptop",
                Price = 50000m,
                SKU = "LAP001",
            },
        };

        var innerResponse = new Response<ICollection<GetProductModel>> { Data = products };
        this.mockInnerService
            .Setup(s => s.GetProductsAsync("usd"))
            .ReturnsAsync(innerResponse);

        this.mockCurrencyService
            .Setup(c => c.Convert(It.IsAny<decimal>(), "PHP", "USD"))
            .Returns(1000m);

        // Act
        var result = await this.decorator.GetProductsAsync("usd");

        // Assert
        Assert.AreEqual("USD", result.Data.First().CurrencyCode);
    }

    [TestMethod]
    public async Task GetProductsAsync_NullData_ReturnsResponseWithoutError()
    {
        // Arrange
        var innerResponse = new Response<ICollection<GetProductModel>> { Data = null! };
        this.mockInnerService
            .Setup(s => s.GetProductsAsync("USD"))
            .ReturnsAsync(innerResponse);

        // Act
        var result = await this.decorator.GetProductsAsync("USD");

        // Assert
        Assert.IsNull(result.Data);
    }

    [TestMethod]
    public async Task GetProductsAsync_MultipleProducts_ConvertsAllPrices()
    {
        // Arrange
        var products = new List<GetProductModel>
        {
            new GetProductModel { Id = Guid.NewGuid(), Name = "Laptop", Price = 50000m, SKU = "LAP001" },
            new GetProductModel { Id = Guid.NewGuid(), Name = "Mouse", Price = 1000m, SKU = "MOU001" },
        };

        var innerResponse = new Response<ICollection<GetProductModel>> { Data = products };
        this.mockInnerService
            .Setup(s => s.GetProductsAsync("USD"))
            .ReturnsAsync(innerResponse);

        this.mockCurrencyService
            .Setup(c => c.Convert(50000m, "PHP", "USD"))
            .Returns(1000m);
        this.mockCurrencyService
            .Setup(c => c.Convert(1000m, "PHP", "USD"))
            .Returns(20m);

        // Act
        var result = await this.decorator.GetProductsAsync("USD");

        // Assert
        Assert.AreEqual(2, result.Data.Count(p => p.ConvertedPrice.HasValue));
    }

    #endregion

    #region GetProductByIdAsync Tests

    [TestMethod]
    public async Task GetProductByIdAsync_NoCurrencySpecified_DelegatesToInnerServiceWithoutConversion()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var product = new GetProductModel
        {
            Id = productId,
            Name = "Laptop",
            Price = 50000m,
            SKU = "LAP001",
        };

        var expectedResponse = new Response<GetProductModel> { Data = product };
        this.mockInnerService
            .Setup(s => s.GetProductByIdAsync(productId, null))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await this.decorator.GetProductByIdAsync(productId, null);

        // Assert
        Assert.AreEqual(expectedResponse, result);
    }

    [TestMethod]
    public async Task GetProductByIdAsync_WithTargetCurrency_AppliesCurrencyConversion()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var product = new GetProductModel
        {
            Id = productId,
            Name = "Laptop",
            Price = 50000m,
            SKU = "LAP001",
        };

        var innerResponse = new Response<GetProductModel> { Data = product };
        this.mockInnerService
            .Setup(s => s.GetProductByIdAsync(productId, "USD"))
            .ReturnsAsync(innerResponse);

        this.mockCurrencyService
            .Setup(c => c.Convert(50000m, "PHP", "USD"))
            .Returns(1000m);

        // Act
        var result = await this.decorator.GetProductByIdAsync(productId, "USD");

        // Assert
        Assert.AreEqual(1000m, result.Data.ConvertedPrice);
    }

    [TestMethod]
    public async Task GetProductByIdAsync_WithTargetCurrency_SetsCurrencyCode()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var product = new GetProductModel
        {
            Id = productId,
            Name = "Laptop",
            Price = 50000m,
            SKU = "LAP001",
        };

        var innerResponse = new Response<GetProductModel> { Data = product };
        this.mockInnerService
            .Setup(s => s.GetProductByIdAsync(productId, "USD"))
            .ReturnsAsync(innerResponse);

        this.mockCurrencyService
            .Setup(c => c.Convert(It.IsAny<decimal>(), It.IsAny<string>(), It.IsAny<string>()))
            .Returns(1000m);

        // Act
        var result = await this.decorator.GetProductByIdAsync(productId, "USD");

        // Assert
        Assert.AreEqual("USD", result.Data.CurrencyCode);
    }

    [TestMethod]
    public async Task GetProductByIdAsync_WithLowercaseCurrency_ConvertsToUppercase()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var product = new GetProductModel
        {
            Id = productId,
            Name = "Laptop",
            Price = 50000m,
            SKU = "LAP001",
        };

        var innerResponse = new Response<GetProductModel> { Data = product };
        this.mockInnerService
            .Setup(s => s.GetProductByIdAsync(productId, "usd"))
            .ReturnsAsync(innerResponse);

        this.mockCurrencyService
            .Setup(c => c.Convert(It.IsAny<decimal>(), "PHP", "USD"))
            .Returns(1000m);

        // Act
        var result = await this.decorator.GetProductByIdAsync(productId, "usd");

        // Assert
        Assert.AreEqual("USD", result.Data.CurrencyCode);
    }

    [TestMethod]
    public async Task GetProductByIdAsync_NullData_ReturnsResponseWithoutError()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var innerResponse = new Response<GetProductModel> { Data = null! };
        this.mockInnerService
            .Setup(s => s.GetProductByIdAsync(productId, "USD"))
            .ReturnsAsync(innerResponse);

        // Act
        var result = await this.decorator.GetProductByIdAsync(productId, "USD");

        // Assert
        Assert.IsNull(result.Data);
    }

    [TestMethod]
    public async Task GetProductByIdAsync_CallsInnerServiceWithCorrectParameters()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var product = new GetProductModel { Id = productId, Name = "Laptop", Price = 50000m };
        var innerResponse = new Response<GetProductModel> { Data = product };

        this.mockInnerService
            .Setup(s => s.GetProductByIdAsync(productId, "USD"))
            .ReturnsAsync(innerResponse);

        this.mockCurrencyService
            .Setup(c => c.Convert(It.IsAny<decimal>(), It.IsAny<string>(), It.IsAny<string>()))
            .Returns(1000m);

        // Act
        await this.decorator.GetProductByIdAsync(productId, "USD");

        // Assert
        this.mockInnerService.Verify(s => s.GetProductByIdAsync(productId, "USD"), Times.Once);
    }

    #endregion

    #region DeleteProductByIdAsync Tests

    [TestMethod]
    public async Task DeleteProductByIdAsync_ValidRequest_DelegatesToInnerService()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var expectedResponse = new Response();

        this.mockInnerService
            .Setup(s => s.DeleteProductByIdAsync(productId, userId))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await this.decorator.DeleteProductByIdAsync(productId, userId);

        // Assert
        Assert.AreEqual(expectedResponse, result);
    }

    [TestMethod]
    public async Task DeleteProductByIdAsync_ValidRequest_CallsInnerServiceOnce()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        this.mockInnerService
            .Setup(s => s.DeleteProductByIdAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
            .ReturnsAsync(new Response());

        // Act
        await this.decorator.DeleteProductByIdAsync(productId, userId);

        // Assert
        this.mockInnerService.Verify(s => s.DeleteProductByIdAsync(productId, userId), Times.Once);
    }

    #endregion

    #region UpdateProductAsync Tests

    [TestMethod]
    public async Task UpdateProductAsync_ValidRequest_DelegatesToInnerService()
    {
        // Arrange
        var request = new UpdateProductRequest
        {
            Id = Guid.NewGuid(),
            Name = "Updated Laptop",
            Price = 1600m,
            UpdatedBy = Guid.NewGuid(),
        };

        var expectedResponse = new Response<UpdateProductResponse>
        {
            Data = new UpdateProductResponse
            {
                ProductId = request.Id.ToString(),
                Name = "Updated Laptop",
                Price = 1600m,
            },
        };

        this.mockInnerService
            .Setup(s => s.UpdateProductAsync(request))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await this.decorator.UpdateProductAsync(request);

        // Assert
        Assert.AreEqual(expectedResponse, result);
    }

    [TestMethod]
    public async Task UpdateProductAsync_ValidRequest_CallsInnerServiceOnce()
    {
        // Arrange
        var request = new UpdateProductRequest
        {
            Id = Guid.NewGuid(),
            Name = "Updated Laptop",
            UpdatedBy = Guid.NewGuid(),
        };

        this.mockInnerService
            .Setup(s => s.UpdateProductAsync(It.IsAny<UpdateProductRequest>()))
            .ReturnsAsync(new Response<UpdateProductResponse>());

        // Act
        await this.decorator.UpdateProductAsync(request);

        // Assert
        this.mockInnerService.Verify(s => s.UpdateProductAsync(request), Times.Once);
    }

    #endregion

    #region GetLookupsAsync Tests

    [TestMethod]
    public async Task GetLookupsAsync_ValidRequest_DelegatesToInnerService()
    {
        // Arrange
        var lookups = new Dictionary<string, IEnumerable<KeyValuePair<string, string>>>
        {
            { "Categories", new List<KeyValuePair<string, string>> { new KeyValuePair<string, string>("1", "Electronics") } },
        };

        var expectedResponse = new Response<Dictionary<string, IEnumerable<KeyValuePair<string, string>>>>(lookups);

        this.mockInnerService
            .Setup(s => s.GetLookupsAsync())
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await this.decorator.GetLookupsAsync();

        // Assert
        Assert.AreEqual(expectedResponse, result);
    }

    [TestMethod]
    public async Task GetLookupsAsync_ValidRequest_CallsInnerServiceOnce()
    {
        // Arrange
        var lookups = new Dictionary<string, IEnumerable<KeyValuePair<string, string>>>();
        var response = new Response<Dictionary<string, IEnumerable<KeyValuePair<string, string>>>>(lookups);

        this.mockInnerService
            .Setup(s => s.GetLookupsAsync())
            .ReturnsAsync(response);

        // Act
        await this.decorator.GetLookupsAsync();

        // Assert
        this.mockInnerService.Verify(s => s.GetLookupsAsync(), Times.Once);
    }

    #endregion
}
