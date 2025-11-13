namespace InventoryDashboard.Services.Product.Tests;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using InventoryDashboard.Infrastructure.Constants.Errors;
using InventoryDashboard.Infrastructure.Entities.Identity;
using InventoryDashboard.Infrastructure.Entities.Products;
using InventoryDashboard.Infrastructure.Logging;
using InventoryDashboard.Infrastructure.Messages.Product;
using InventoryDashboard.Infrastructure.Services.Interfaces;
using InventoryDashboard.Services.Identity.Data;
using InventoryDashboard.Services.Product.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

[TestClass]
public class ProductServiceTest
{
    private ProductDbContext productDbContext = null!;
    private ApplicationDbContext applicationDbContext = null!;
    private Mock<IValidator<AddProductRequest>> mockAddProductValidator = null!;
    private Mock<ILogger> mockLogger = null!;
    private Mock<ICurrencyService> mockCurrencyService = null!;
    private ProductService productService = null!;

    [TestInitialize]
    public void Setup()
    {
        // Arrange - Setup in-memory databases
        var productDbOptions = new DbContextOptionsBuilder<ProductDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var applicationDbOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        this.productDbContext = new ProductDbContext(productDbOptions);
        this.applicationDbContext = new ApplicationDbContext(applicationDbOptions);
        this.mockAddProductValidator = new Mock<IValidator<AddProductRequest>>();
        this.mockLogger = new Mock<ILogger>();
        this.mockCurrencyService = new Mock<ICurrencyService>();

        this.productService = new ProductService(
            this.productDbContext,
            this.applicationDbContext,
            this.mockAddProductValidator.Object,
            this.mockLogger.Object,
            this.mockCurrencyService.Object);
    }

    [TestCleanup]
    public void Cleanup()
    {
        this.productDbContext?.Dispose();
        this.applicationDbContext?.Dispose();
    }

    #region AddProductAsync Tests

    [TestMethod]
    public async Task AddProductAsync_ValidRequest_ReturnsSuccessResponse()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var category = new Category { Id = categoryId, Name = "Electronics" };
        await this.productDbContext.Categories.AddAsync(category);
        await this.productDbContext.SaveChangesAsync();

        var user = new User { Id = userId, UserName = "testuser" };
        await this.applicationDbContext.Users.AddAsync(user);
        await this.applicationDbContext.SaveChangesAsync();

        var request = new AddProductRequest
        {
            Name = "Laptop",
            Description = "Gaming Laptop",
            CategoryId = categoryId,
            Price = 1500.00m,
            StockQuantity = 10,
            SKU = "LAP001",
            CreatedBy = userId,
            UpdatedBy = userId,
        };

        this.mockAddProductValidator
            .Setup(v => v.ValidateAsync(It.IsAny<AddProductRequest>(), default))
            .ReturnsAsync(new ValidationResult());

        // Act
        var result = await this.productService.AddProductAsync(request);

        // Assert
        Assert.IsNotNull(result.Data);
    }

    [TestMethod]
    public async Task AddProductAsync_ValidRequest_ProductIsAddedToDatabase()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var category = new Category { Id = categoryId, Name = "Electronics" };
        await this.productDbContext.Categories.AddAsync(category);
        await this.productDbContext.SaveChangesAsync();

        var user = new User { Id = userId, UserName = "testuser" };
        await this.applicationDbContext.Users.AddAsync(user);
        await this.applicationDbContext.SaveChangesAsync();

        var request = new AddProductRequest
        {
            Name = "Laptop",
            Description = "Gaming Laptop",
            CategoryId = categoryId,
            Price = 1500.00m,
            StockQuantity = 10,
            SKU = "LAP001",
            CreatedBy = userId,
            UpdatedBy = userId,
        };

        this.mockAddProductValidator
            .Setup(v => v.ValidateAsync(It.IsAny<AddProductRequest>(), default))
            .ReturnsAsync(new ValidationResult());

        // Act
        await this.productService.AddProductAsync(request);

        // Assert
        var productInDb = await this.productDbContext.Products.FirstOrDefaultAsync(p => p.SKU == "LAP001");
        Assert.IsNotNull(productInDb);
    }

    [TestMethod]
    public async Task AddProductAsync_ValidRequest_ReturnsCorrectProductName()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var category = new Category { Id = categoryId, Name = "Electronics" };
        await this.productDbContext.Categories.AddAsync(category);
        await this.productDbContext.SaveChangesAsync();

        var user = new User { Id = userId, UserName = "testuser" };
        await this.applicationDbContext.Users.AddAsync(user);
        await this.applicationDbContext.SaveChangesAsync();

        var request = new AddProductRequest
        {
            Name = "Laptop",
            Description = "Gaming Laptop",
            CategoryId = categoryId,
            Price = 1500.00m,
            StockQuantity = 10,
            SKU = "LAP001",
            CreatedBy = userId,
            UpdatedBy = userId,
        };

        this.mockAddProductValidator
            .Setup(v => v.ValidateAsync(It.IsAny<AddProductRequest>(), default))
            .ReturnsAsync(new ValidationResult());

        // Act
        var result = await this.productService.AddProductAsync(request);

        // Assert
        Assert.AreEqual("Laptop", result.Data.Name);
    }

    [TestMethod]
    public async Task AddProductAsync_DuplicateSKU_ReturnsProductAlreadyExistsError()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var category = new Category { Id = categoryId, Name = "Electronics" };
        await this.productDbContext.Categories.AddAsync(category);

        var existingProduct = new Product
        {
            Id = Guid.NewGuid(),
            Name = "Existing Laptop",
            SKU = "LAP001",
            Price = 1000m,
            StockQuantity = 5,
            Category = category,
            CreatedBy = userId,
            UpdatedBy = userId,
        };
        await this.productDbContext.Products.AddAsync(existingProduct);
        await this.productDbContext.SaveChangesAsync();

        var request = new AddProductRequest
        {
            Name = "New Laptop",
            Description = "New Gaming Laptop",
            CategoryId = categoryId,
            Price = 1500.00m,
            StockQuantity = 10,
            SKU = "LAP001",
            CreatedBy = userId,
            UpdatedBy = userId,
        };

        // Act
        var result = await this.productService.AddProductAsync(request);

        // Assert
        Assert.AreEqual(ProductServiceErrorCodes.ProductAlreadyExists, result.ErrorCode);
    }

    [TestMethod]
    public async Task AddProductAsync_UserNotFound_ReturnsAuthenticationFailedError()
    {
        // Arrange
        var nonExistentUserId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var category = new Category { Id = categoryId, Name = "Electronics" };
        await this.productDbContext.Categories.AddAsync(category);
        await this.productDbContext.SaveChangesAsync();

        var request = new AddProductRequest
        {
            Name = "Laptop",
            Description = "Gaming Laptop",
            CategoryId = categoryId,
            Price = 1500.00m,
            StockQuantity = 10,
            SKU = "LAP001",
            CreatedBy = nonExistentUserId,
            UpdatedBy = nonExistentUserId,
        };

        // Act
        var result = await this.productService.AddProductAsync(request);

        // Assert
        Assert.AreEqual(ProductServiceErrorCodes.AuthenticationFailed, result.ErrorCode);
    }

    [TestMethod]
    public async Task AddProductAsync_ValidationFails_ReturnsValidationError()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var user = new User { Id = userId, UserName = "testuser" };
        await this.applicationDbContext.Users.AddAsync(user);
        await this.applicationDbContext.SaveChangesAsync();

        var request = new AddProductRequest
        {
            Name = string.Empty,
            Description = "Gaming Laptop",
            CategoryId = categoryId,
            Price = 1500.00m,
            StockQuantity = 10,
            SKU = "LAP001",
            CreatedBy = userId,
            UpdatedBy = userId,
        };

        var validationFailures = new List<ValidationFailure>
        {
            new ValidationFailure("Name", "Name is required"),
        };
        this.mockAddProductValidator
            .Setup(v => v.ValidateAsync(It.IsAny<AddProductRequest>(), default))
            .ReturnsAsync(new ValidationResult(validationFailures));

        // Act
        var result = await this.productService.AddProductAsync(request);

        // Assert
        Assert.AreEqual(ProductServiceErrorCodes.ValidationError, result.ErrorCode);
    }

    [TestMethod]
    public async Task AddProductAsync_CategoryNotFound_ReturnsLookupDataNotFoundError()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var nonExistentCategoryId = Guid.NewGuid();
        var user = new User { Id = userId, UserName = "testuser" };
        await this.applicationDbContext.Users.AddAsync(user);
        await this.applicationDbContext.SaveChangesAsync();

        var request = new AddProductRequest
        {
            Name = "Laptop",
            Description = "Gaming Laptop",
            CategoryId = nonExistentCategoryId,
            Price = 1500.00m,
            StockQuantity = 10,
            SKU = "LAP001",
            CreatedBy = userId,
            UpdatedBy = userId,
        };

        this.mockAddProductValidator
            .Setup(v => v.ValidateAsync(It.IsAny<AddProductRequest>(), default))
            .ReturnsAsync(new ValidationResult());

        // Act
        var result = await this.productService.AddProductAsync(request);

        // Assert
        Assert.AreEqual(ProductServiceErrorCodes.LookupDataNotFound, result.ErrorCode);
    }

    #endregion

    #region GetProductsAsync Tests

    [TestMethod]
    public async Task GetProductsAsync_NoProducts_ReturnsEmptyCollection()
    {
        // Arrange & Act
        var result = await this.productService.GetProductsAsync();

        // Assert
        Assert.AreEqual(0, result.Data.Count);
    }

    [TestMethod]
    public async Task GetProductsAsync_ProductsExist_ReturnsAllNonDeletedProducts()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var category = new Category { Id = categoryId, Name = "Electronics" };
        await this.productDbContext.Categories.AddAsync(category);

        var product1 = new Product
        {
            Id = Guid.NewGuid(),
            Name = "Laptop",
            SKU = "LAP001",
            Price = 1000m,
            StockQuantity = 5,
            Category = category,
            IsDeleted = false,
            CreatedBy = userId,
            UpdatedBy = userId,
        };
        var product2 = new Product
        {
            Id = Guid.NewGuid(),
            Name = "Mouse",
            SKU = "MOU001",
            Price = 50m,
            StockQuantity = 20,
            Category = category,
            IsDeleted = false,
            CreatedBy = userId,
            UpdatedBy = userId,
        };
        await this.productDbContext.Products.AddRangeAsync(product1, product2);
        await this.productDbContext.SaveChangesAsync();

        // Act
        var result = await this.productService.GetProductsAsync();

        // Assert
        Assert.AreEqual(2, result.Data.Count);
    }

    [TestMethod]
    public async Task GetProductsAsync_WithDeletedProducts_ReturnsOnlyNonDeletedProducts()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var category = new Category { Id = categoryId, Name = "Electronics" };
        await this.productDbContext.Categories.AddAsync(category);

        var product1 = new Product
        {
            Id = Guid.NewGuid(),
            Name = "Laptop",
            SKU = "LAP001",
            Price = 1000m,
            StockQuantity = 5,
            Category = category,
            IsDeleted = false,
            CreatedBy = userId,
            UpdatedBy = userId,
        };
        var deletedProduct = new Product
        {
            Id = Guid.NewGuid(),
            Name = "Old Mouse",
            SKU = "MOU001",
            Price = 50m,
            StockQuantity = 0,
            Category = category,
            IsDeleted = true,
            CreatedBy = userId,
            UpdatedBy = userId,
        };
        await this.productDbContext.Products.AddRangeAsync(product1, deletedProduct);
        await this.productDbContext.SaveChangesAsync();

        // Act
        var result = await this.productService.GetProductsAsync();

        // Assert
        Assert.AreEqual(1, result.Data.Count);
    }

    [TestMethod]
    public async Task GetProductsAsync_UnsupportedCurrency_ReturnsCurrencyCodeNotSupportedError()
    {
        // Arrange
        this.mockCurrencyService
            .Setup(c => c.IsSupportedCurrency("INVALID"))
            .Returns(false);

        // Act
        var result = await this.productService.GetProductsAsync("INVALID");

        // Assert
        Assert.AreEqual(ProductServiceErrorCodes.CurrencyCodeNotSupported, result.ErrorCode);
    }

    [TestMethod]
    public async Task GetProductsAsync_ValidCurrency_DoesNotReturnError()
    {
        // Arrange
        this.mockCurrencyService
            .Setup(c => c.IsSupportedCurrency("USD"))
            .Returns(true);

        // Act
        var result = await this.productService.GetProductsAsync("USD");

        // Assert
        Assert.AreEqual(string.Empty, result.ErrorCode);
    }

    #endregion

    #region GetProductByIdAsync Tests

    [TestMethod]
    public async Task GetProductByIdAsync_ProductExists_ReturnsProduct()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var category = new Category { Id = categoryId, Name = "Electronics" };
        await this.productDbContext.Categories.AddAsync(category);

        var product = new Product
        {
            Id = productId,
            Name = "Laptop",
            SKU = "LAP001",
            Price = 1000m,
            StockQuantity = 5,
            Category = category,
            IsDeleted = false,
            CreatedBy = userId,
            UpdatedBy = userId,
        };
        await this.productDbContext.Products.AddAsync(product);
        await this.productDbContext.SaveChangesAsync();

        // Act
        var result = await this.productService.GetProductByIdAsync(productId);

        // Assert
        Assert.IsNotNull(result.Data);
    }

    [TestMethod]
    public async Task GetProductByIdAsync_ProductExists_ReturnsCorrectProductId()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var category = new Category { Id = categoryId, Name = "Electronics" };
        await this.productDbContext.Categories.AddAsync(category);

        var product = new Product
        {
            Id = productId,
            Name = "Laptop",
            SKU = "LAP001",
            Price = 1000m,
            StockQuantity = 5,
            Category = category,
            IsDeleted = false,
            CreatedBy = userId,
            UpdatedBy = userId,
        };
        await this.productDbContext.Products.AddAsync(product);
        await this.productDbContext.SaveChangesAsync();

        // Act
        var result = await this.productService.GetProductByIdAsync(productId);

        // Assert
        Assert.AreEqual(productId, result.Data.Id);
    }

    [TestMethod]
    public async Task GetProductByIdAsync_ProductNotFound_ReturnsProductNotFoundError()
    {
        // Arrange
        var nonExistentProductId = Guid.NewGuid();

        // Act
        var result = await this.productService.GetProductByIdAsync(nonExistentProductId);

        // Assert
        Assert.AreEqual(ProductServiceErrorCodes.ProductNotFound, result.ErrorCode);
    }

    [TestMethod]
    public async Task GetProductByIdAsync_DeletedProduct_ReturnsProductNotFoundError()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var category = new Category { Id = categoryId, Name = "Electronics" };
        await this.productDbContext.Categories.AddAsync(category);

        var deletedProduct = new Product
        {
            Id = productId,
            Name = "Laptop",
            SKU = "LAP001",
            Price = 1000m,
            StockQuantity = 5,
            Category = category,
            IsDeleted = true,
            CreatedBy = userId,
            UpdatedBy = userId,
        };
        await this.productDbContext.Products.AddAsync(deletedProduct);
        await this.productDbContext.SaveChangesAsync();

        // Act
        var result = await this.productService.GetProductByIdAsync(productId);

        // Assert
        Assert.AreEqual(ProductServiceErrorCodes.ProductNotFound, result.ErrorCode);
    }

    [TestMethod]
    public async Task GetProductByIdAsync_UnsupportedCurrency_ReturnsCurrencyCodeNotSupportedError()
    {
        // Arrange
        var productId = Guid.NewGuid();
        this.mockCurrencyService
            .Setup(c => c.IsSupportedCurrency("INVALID"))
            .Returns(false);

        // Act
        var result = await this.productService.GetProductByIdAsync(productId, "INVALID");

        // Assert
        Assert.AreEqual(ProductServiceErrorCodes.CurrencyCodeNotSupported, result.ErrorCode);
    }

    #endregion

    #region DeleteProductByIdAsync Tests

    [TestMethod]
    public async Task DeleteProductByIdAsync_ProductExists_ReturnsSuccessResponse()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var category = new Category { Id = categoryId, Name = "Electronics" };
        await this.productDbContext.Categories.AddAsync(category);

        var product = new Product
        {
            Id = productId,
            Name = "Laptop",
            SKU = "LAP001",
            Price = 1000m,
            StockQuantity = 5,
            Category = category,
            IsDeleted = false,
            CreatedBy = userId,
            UpdatedBy = userId,
        };
        await this.productDbContext.Products.AddAsync(product);
        await this.productDbContext.SaveChangesAsync();

        // Act
        var result = await this.productService.DeleteProductByIdAsync(productId, userId);

        // Assert
        Assert.AreEqual(string.Empty, result.ErrorCode);
    }

    [TestMethod]
    public async Task DeleteProductByIdAsync_ProductExists_MarksProductAsDeleted()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var category = new Category { Id = categoryId, Name = "Electronics" };
        await this.productDbContext.Categories.AddAsync(category);

        var product = new Product
        {
            Id = productId,
            Name = "Laptop",
            SKU = "LAP001",
            Price = 1000m,
            StockQuantity = 5,
            Category = category,
            IsDeleted = false,
            CreatedBy = userId,
            UpdatedBy = userId,
        };
        await this.productDbContext.Products.AddAsync(product);
        await this.productDbContext.SaveChangesAsync();

        // Act
        await this.productService.DeleteProductByIdAsync(productId, userId);

        // Assert
        var deletedProduct = await this.productDbContext.Products.FindAsync(productId);
        Assert.IsTrue(deletedProduct!.IsDeleted);
    }

    [TestMethod]
    public async Task DeleteProductByIdAsync_ProductNotFound_ReturnsProductNotFoundError()
    {
        // Arrange
        var nonExistentProductId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        // Act
        var result = await this.productService.DeleteProductByIdAsync(nonExistentProductId, userId);

        // Assert
        Assert.AreEqual(ProductServiceErrorCodes.ProductNotFound, result.ErrorCode);
    }

    [TestMethod]
    public async Task DeleteProductByIdAsync_AlreadyDeletedProduct_ReturnsProductNotFoundError()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var category = new Category { Id = categoryId, Name = "Electronics" };
        await this.productDbContext.Categories.AddAsync(category);

        var deletedProduct = new Product
        {
            Id = productId,
            Name = "Laptop",
            SKU = "LAP001",
            Price = 1000m,
            StockQuantity = 5,
            Category = category,
            IsDeleted = true,
            CreatedBy = userId,
            UpdatedBy = userId,
        };
        await this.productDbContext.Products.AddAsync(deletedProduct);
        await this.productDbContext.SaveChangesAsync();

        // Act
        var result = await this.productService.DeleteProductByIdAsync(productId, userId);

        // Assert
        Assert.AreEqual(ProductServiceErrorCodes.ProductNotFound, result.ErrorCode);
    }

    #endregion

    #region UpdateProductAsync Tests

    [TestMethod]
    public async Task UpdateProductAsync_ValidRequest_ReturnsSuccessResponse()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var category = new Category { Id = categoryId, Name = "Electronics" };
        await this.productDbContext.Categories.AddAsync(category);

        var product = new Product
        {
            Id = productId,
            Name = "Laptop",
            SKU = "LAP001",
            Price = 1000m,
            StockQuantity = 5,
            Category = category,
            IsDeleted = false,
            CreatedBy = userId,
            UpdatedBy = userId,
        };
        await this.productDbContext.Products.AddAsync(product);
        await this.productDbContext.SaveChangesAsync();

        var user = new User { Id = userId, UserName = "testuser" };
        await this.applicationDbContext.Users.AddAsync(user);
        await this.applicationDbContext.SaveChangesAsync();

        var updateRequest = new UpdateProductRequest
        {
            Id = productId,
            Name = "Updated Laptop",
            Price = 1200m,
            UpdatedBy = userId,
        };

        // Act
        var result = await this.productService.UpdateProductAsync(updateRequest);

        // Assert
        Assert.IsNotNull(result.Data);
    }

    [TestMethod]
    public async Task UpdateProductAsync_ValidRequest_UpdatesProductName()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var category = new Category { Id = categoryId, Name = "Electronics" };
        await this.productDbContext.Categories.AddAsync(category);

        var product = new Product
        {
            Id = productId,
            Name = "Laptop",
            SKU = "LAP001",
            Price = 1000m,
            StockQuantity = 5,
            Category = category,
            IsDeleted = false,
            CreatedBy = userId,
            UpdatedBy = userId,
        };
        await this.productDbContext.Products.AddAsync(product);
        await this.productDbContext.SaveChangesAsync();

        var user = new User { Id = userId, UserName = "testuser" };
        await this.applicationDbContext.Users.AddAsync(user);
        await this.applicationDbContext.SaveChangesAsync();

        var updateRequest = new UpdateProductRequest
        {
            Id = productId,
            Name = "Updated Laptop",
            UpdatedBy = userId,
        };

        // Act
        await this.productService.UpdateProductAsync(updateRequest);

        // Assert
        var updatedProduct = await this.productDbContext.Products.FindAsync(productId);
        Assert.AreEqual("Updated Laptop", updatedProduct!.Name);
    }

    [TestMethod]
    public async Task UpdateProductAsync_ProductNotFound_ReturnsProductNotFoundError()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var nonExistentProductId = Guid.NewGuid();

        var user = new User { Id = userId, UserName = "testuser" };
        await this.applicationDbContext.Users.AddAsync(user);
        await this.applicationDbContext.SaveChangesAsync();

        var updateRequest = new UpdateProductRequest
        {
            Id = nonExistentProductId,
            Name = "Updated Laptop",
            UpdatedBy = userId,
        };

        // Act
        var result = await this.productService.UpdateProductAsync(updateRequest);

        // Assert
        Assert.AreEqual(ProductServiceErrorCodes.ProductNotFound, result.ErrorCode);
    }

    [TestMethod]
    public async Task UpdateProductAsync_UserNotFound_ReturnsAuthenticationFailedError()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var category = new Category { Id = categoryId, Name = "Electronics" };
        await this.productDbContext.Categories.AddAsync(category);

        var product = new Product
        {
            Id = productId,
            Name = "Laptop",
            SKU = "LAP001",
            Price = 1000m,
            StockQuantity = 5,
            Category = category,
            IsDeleted = false,
            CreatedBy = userId,
            UpdatedBy = userId,
        };
        await this.productDbContext.Products.AddAsync(product);
        await this.productDbContext.SaveChangesAsync();

        var nonExistentUserId = Guid.NewGuid();
        var updateRequest = new UpdateProductRequest
        {
            Id = productId,
            Name = "Updated Laptop",
            UpdatedBy = nonExistentUserId,
        };

        // Act
        var result = await this.productService.UpdateProductAsync(updateRequest);

        // Assert
        Assert.AreEqual(ProductServiceErrorCodes.AuthenticationFailed, result.ErrorCode);
    }

    [TestMethod]
    public async Task UpdateProductAsync_InvalidCategory_ReturnsLookupDataNotFoundError()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var category = new Category { Id = categoryId, Name = "Electronics" };
        await this.productDbContext.Categories.AddAsync(category);

        var product = new Product
        {
            Id = productId,
            Name = "Laptop",
            SKU = "LAP001",
            Price = 1000m,
            StockQuantity = 5,
            Category = category,
            IsDeleted = false,
            CreatedBy = userId,
            UpdatedBy = userId,
        };
        await this.productDbContext.Products.AddAsync(product);
        await this.productDbContext.SaveChangesAsync();

        var user = new User { Id = userId, UserName = "testuser" };
        await this.applicationDbContext.Users.AddAsync(user);
        await this.applicationDbContext.SaveChangesAsync();

        var nonExistentCategoryId = Guid.NewGuid();
        var updateRequest = new UpdateProductRequest
        {
            Id = productId,
            CategoryId = nonExistentCategoryId,
            UpdatedBy = userId,
        };

        // Act
        var result = await this.productService.UpdateProductAsync(updateRequest);

        // Assert
        Assert.AreEqual(ProductServiceErrorCodes.LookupDataNotFound, result.ErrorCode);
    }

    #endregion

    #region GetLookupsAsync Tests

    [TestMethod]
    public async Task GetLookupsAsync_NoCategories_ReturnsEmptyCategoriesList()
    {
        // Arrange & Act
        var result = await this.productService.GetLookupsAsync();

        // Assert
        Assert.AreEqual(0, result.Data["Categories"].Count());
    }

    [TestMethod]
    public async Task GetLookupsAsync_CategoriesExist_ReturnsAllCategories()
    {
        // Arrange
        var category1 = new Category { Id = Guid.NewGuid(), Name = "Electronics" };
        var category2 = new Category { Id = Guid.NewGuid(), Name = "Clothing" };
        await this.productDbContext.Categories.AddRangeAsync(category1, category2);
        await this.productDbContext.SaveChangesAsync();

        // Act
        var result = await this.productService.GetLookupsAsync();

        // Assert
        Assert.AreEqual(2, result.Data["Categories"].Count());
    }

    [TestMethod]
    public async Task GetLookupsAsync_CategoriesExist_ReturnsCategoriesWithCorrectKeys()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var category = new Category { Id = categoryId, Name = "Electronics" };
        await this.productDbContext.Categories.AddAsync(category);
        await this.productDbContext.SaveChangesAsync();

        // Act
        var result = await this.productService.GetLookupsAsync();

        // Assert
        var firstCategory = result.Data["Categories"].First();
        Assert.AreEqual(categoryId.ToString(), firstCategory.Key);
    }

    [TestMethod]
    public async Task GetLookupsAsync_CategoriesExist_ReturnsCategoriesWithCorrectValues()
    {
        // Arrange
        var category = new Category { Id = Guid.NewGuid(), Name = "Electronics" };
        await this.productDbContext.Categories.AddAsync(category);
        await this.productDbContext.SaveChangesAsync();

        // Act
        var result = await this.productService.GetLookupsAsync();

        // Assert
        var firstCategory = result.Data["Categories"].First();
        Assert.AreEqual("Electronics", firstCategory.Value);
    }

    #endregion
}
