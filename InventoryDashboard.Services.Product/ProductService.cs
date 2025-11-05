namespace InventoryDashboard.Services.Product
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using FluentValidation;
    using InventoryDashboard.Infrastructure.Constants.Errors;
    using InventoryDashboard.Infrastructure.Entities.Products;
    using InventoryDashboard.Infrastructure.Logging;
    using InventoryDashboard.Infrastructure.Messages;
    using InventoryDashboard.Infrastructure.Messages.Product;
    using InventoryDashboard.Infrastructure.Models.Products;
    using InventoryDashboard.Infrastructure.Services.Interfaces;
    using InventoryDashboard.Services.Identity.Data;
    using InventoryDashboard.Services.Product.Data;
    using Microsoft.EntityFrameworkCore;

    public class ProductService : IProductService
    {
        private readonly ProductDbContext productDbContext;
        private readonly ApplicationDbContext aplicationDbContext;
        private readonly IValidator<AddProductRequest> addProductValidator;
        private readonly ILogger logger;
        private readonly ICurrencyService currencyService;

        public ProductService(ProductDbContext productDbContext, ApplicationDbContext aplicationDbContext, IValidator<AddProductRequest> addProductValidator, ILogger logger, ICurrencyService currencyService)
        {
            this.productDbContext = productDbContext;
            this.aplicationDbContext = aplicationDbContext;
            this.addProductValidator = addProductValidator;
            this.logger = logger;
            this.currencyService = currencyService;
        }

        public async Task<Response<AddProductResponse>> AddProductAsync(AddProductRequest request)
        {
            try
            {
                var existingProduct = await this.productDbContext.Products
                    .FirstOrDefaultAsync(p => p.SKU == request.SKU);

                if (existingProduct != null)
                {
                    var duplicateResponse = new Response<AddProductResponse>();
                    duplicateResponse.SetError(ProductServiceErrorCodes.ProductAlreadyExists, $"A product with the SKU '{request.SKU}' already exists.");
                    return duplicateResponse;
                }

                var existingUser = await this.aplicationDbContext.Users
                    .FirstOrDefaultAsync(u => u.Id == request.CreatedBy);

                if (existingUser == null)
                {
                    var userResponse = new Response<AddProductResponse>();
                    userResponse.SetError(ProductServiceErrorCodes.AuthenticationFailed, "User not found.");
                    return userResponse;
                }

                var validationResult = await this.addProductValidator.ValidateAsync(request);
                if (!validationResult.IsValid)
                {
                    var errorMessages = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
                    this.logger.StackTrace("ProductValidationFailed", new Dictionary<string, string>
                    {
                        { "ValidationErrors", errorMessages },
                        { "SKU", request.SKU ?? "Unknown" },
                    });

                    var validationResponse = new Response<AddProductResponse>();
                    validationResponse.SetError(ProductServiceErrorCodes.ValidationError, $"Validation failed: {errorMessages}");
                    return validationResponse;
                }

                var category = await this.productDbContext.Categories.FindAsync(request.CategoryId);
                if (category == null)
                {
                    var catResponse = new Response<AddProductResponse>();
                    catResponse.SetError(ProductServiceErrorCodes.LookupDataNotFound, "Category not found.");
                    return catResponse;
                }

                var product = new Product
                {
                    Id = Guid.NewGuid(),
                    Name = request.Name,
                    Description = request.Description,
                    Category = category,
                    Price = request.Price,
                    StockQuantity = request.StockQuantity,
                    SKU = request.SKU,
                    CreatedAt = DateTime.UtcNow,
                    IsDeleted = false,
                    UpdatedAt = DateTime.UtcNow,
                    CreatedBy = request.CreatedBy,
                    UpdatedBy = request.UpdatedBy,
                };
                await this.productDbContext.Products.AddAsync(product);
                await this.productDbContext.SaveChangesAsync();

                var success = new Response<AddProductResponse>();
                success.Data = new AddProductResponse
                {
                    ProductId = product.Id.ToString(),
                    Name = product.Name,
                    Description = product.Description ?? string.Empty,
                    CategoryId = product.Category?.Id.ToString() ?? string.Empty,
                    Price = product.Price,
                    StockQuantity = product.StockQuantity,
                    SKU = product.SKU,
                };

                return success;
            }
            catch (Exception ex)
            {
                this.logger.StackTrace("ProductServiceException", new Dictionary<string, string> { { "Message", ex.Message } });
                var err = new Response<AddProductResponse>();
                err.SetError(ProductServiceErrorCodes.UnexpectedError, "An unexpected error occurred.");
                return err;
            }
        }

        public async Task<Response<ICollection<GetProductModel>>> GetProductsAsync(string targetCurrency = null)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(targetCurrency) && !this.currencyService.IsSupportedCurrency(targetCurrency))
                {
                    var currencyError = new Response<ICollection<GetProductModel>>();
                    currencyError.SetError(ProductServiceErrorCodes.CurrencyCodeNotSupported, $"The currency code '{targetCurrency}' is not supported.");
                    return currencyError;
                }

                var products = await this.productDbContext.Products
                    .Include(p => p.Category)
                    .Where(p => !p.IsDeleted)
                    .ToListAsync();

                var resultList = new List<GetProductModel>();

                foreach (var p in products)
                {
                    var model = new GetProductModel
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Description = p.Description ?? string.Empty,
                        Category = p.Category,
                        Price = p.Price,
                        StockQuantity = p.StockQuantity,
                        SKU = p.SKU,
                        CreatedAt = p.CreatedAt,
                        UpdatedAt = p.UpdatedAt,
                        CreatedBy = p.CreatedBy,
                        UpdatedBy = p.UpdatedBy,
                    };

                    resultList.Add(model);
                }

                var response = new Response<ICollection<GetProductModel>>();
                response.Data = resultList;
                return response;
            }
            catch (Exception ex)
            {
                this.logger.StackTrace("GetProductsException", new Dictionary<string, string> { { "Message", ex.Message } });
                var err = new Response<ICollection<GetProductModel>>();
                err.SetError(ProductServiceErrorCodes.UnexpectedError, "An unexpected error occurred.");
                return err;
            }
        }

        public async Task<Response<GetProductModel>> GetProductByIdAsync(Guid id, string targetCurrency = null)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(targetCurrency) && !this.currencyService.IsSupportedCurrency(targetCurrency))
                {
                    var currencyError = new Response<GetProductModel>();
                    currencyError.SetError(ProductServiceErrorCodes.CurrencyCodeNotSupported, $"The currency code '{targetCurrency}' is not supported.");
                    return currencyError;
                }

                var productEntity = await this.productDbContext.Products
                    .Include(p => p.Category)
                    .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);

                if (productEntity == null)
                {
                    var notFound = new Response<GetProductModel>();
                    notFound.SetError(ProductServiceErrorCodes.ProductNotFound, "Product not found.");
                    return notFound;
                }

                var model = new GetProductModel
                {
                    Id = productEntity.Id,
                    Name = productEntity.Name,
                    Description = productEntity.Description ?? string.Empty,
                    Category = productEntity.Category,
                    Price = productEntity.Price,
                    StockQuantity = productEntity.StockQuantity,
                    SKU = productEntity.SKU,
                    CreatedAt = productEntity.CreatedAt,
                    UpdatedAt = productEntity.UpdatedAt,
                    CreatedBy = productEntity.CreatedBy,
                    UpdatedBy = productEntity.UpdatedBy,
                };

                var response = new Response<GetProductModel>();
                response.Data = model;
                return response;
            }
            catch (Exception ex)
            {
                this.logger.StackTrace("GetProductsException", new Dictionary<string, string> { { "Message", ex.Message } });
                var err = new Response<GetProductModel>();
                err.SetError(ProductServiceErrorCodes.UnexpectedError, "An unexpected error occurred.");
                return err;
            }
        }

        public async Task<Response> DeleteProductByIdAsync(Guid productId, Guid currentUserId)
        {
            try
            {
                var product = await this.productDbContext.Products
                    .FirstOrDefaultAsync(p => p.Id == productId && !p.IsDeleted);

                if (product == null)
                {
                    var notFound = new Response();
                    notFound.SetError(ProductServiceErrorCodes.ProductNotFound, "Product not found.");
                    return notFound;
                }

                product.IsDeleted = true;
                product.UpdatedAt = DateTime.UtcNow;
                product.UpdatedBy = currentUserId;

                this.productDbContext.Products.Update(product);
                await this.productDbContext.SaveChangesAsync();

                return new Response();
            }
            catch (Exception ex)
            {
                this.logger.StackTrace("DeleteProductException", new Dictionary<string, string> { { "Message", ex.Message } });
                var err = new Response();
                err.SetError(ProductServiceErrorCodes.UnexpectedError, "An unexpected error occurred.");
                return err;
            }
        }

        public async Task<Response<UpdateProductResponse>> UpdateProductAsync(UpdateProductRequest request)
        {
            try
            {
                var existingProduct = await this.productDbContext.Products
                    .FirstOrDefaultAsync(p => p.Id == request.Id && !p.IsDeleted);

                if (existingProduct == null)
                {
                    var notFoundResponse = new Response<UpdateProductResponse>();
                    notFoundResponse.SetError(ProductServiceErrorCodes.ProductNotFound, $"Product with ID '{request.Id}' not found.");
                    return notFoundResponse;
                }

                var existingUser = await this.aplicationDbContext.Users
                    .FirstOrDefaultAsync(u => u.Id == request.UpdatedBy);

                if (existingUser == null)
                {
                    var userResponse = new Response<UpdateProductResponse>();
                    userResponse.SetError(ProductServiceErrorCodes.AuthenticationFailed, "User not found.");
                    return userResponse;
                }

                if (!string.IsNullOrWhiteSpace(request.Name))
                {
                    existingProduct.Name = request.Name;
                }

                if (!string.IsNullOrWhiteSpace(request.Description))
                {
                    existingProduct.Description = request.Description;
                }

                if (request.CategoryId.HasValue && request.CategoryId.Value != Guid.Empty)
                {
                    var category = await this.productDbContext.Categories.FindAsync(request.CategoryId.Value);
                    if (category == null)
                    {
                        var catResponse = new Response<UpdateProductResponse>();
                        catResponse.SetError(ProductServiceErrorCodes.LookupDataNotFound, "Category not found.");
                        return catResponse;
                    }

                    existingProduct.Category = category;
                }

                if (request.Price.HasValue)
                {
                    existingProduct.Price = request.Price.Value;
                }

                if (request.StockQuantity.HasValue)
                {
                    existingProduct.StockQuantity = request.StockQuantity.Value;
                }

                if (!string.IsNullOrWhiteSpace(request.SKU))
                {
                    existingProduct.SKU = request.SKU;
                }

                existingProduct.UpdatedAt = DateTime.UtcNow;
                existingProduct.UpdatedBy = request.UpdatedBy;

                this.productDbContext.Products.Update(existingProduct);
                await this.productDbContext.SaveChangesAsync();

                var success = new Response<UpdateProductResponse>();
                success.Data = new UpdateProductResponse
                {
                    ProductId = existingProduct.Id.ToString(),
                    Name = existingProduct.Name,
                    Description = existingProduct.Description ?? string.Empty,
                    CategoryId = existingProduct.Category?.Id.ToString() ?? string.Empty,
                    Price = existingProduct.Price,
                    StockQuantity = existingProduct.StockQuantity,
                    SKU = existingProduct.SKU,
                };

                return success;
            }
            catch (Exception ex)
            {
                this.logger.StackTrace("UpdateProductException", new Dictionary<string, string> { { "Message", ex.Message } });
                var err = new Response<UpdateProductResponse>();
                err.SetError(ProductServiceErrorCodes.UnexpectedError, "An unexpected error occurred.");
                return err;
            }
        }

        public async Task<Response<Dictionary<string, IEnumerable<KeyValuePair<string, string>>>>> GetLookupsAsync()
        {
            try
            {
                var categories = await this.productDbContext.Categories
                    .Select(c => new KeyValuePair<string, string>(c.Id.ToString(), c.Name))
                    .ToListAsync();

                var lookupTables = new Dictionary<string, IEnumerable<KeyValuePair<string, string>>>
                {
                    { "Categories", categories },
                };

                var response = new Response<Dictionary<string, IEnumerable<KeyValuePair<string, string>>>>(lookupTables);
                return response;
            }
            catch (Exception ex)
            {
                this.logger.StackTrace("GetLookupsException", new Dictionary<string, string> { { "Message", ex.Message } });
                var err = new Response<Dictionary<string, IEnumerable<KeyValuePair<string, string>>>>();
                err.SetError(ProductServiceErrorCodes.UnexpectedError, "An unexpected error occurred.");
                return err;
            }
        }
    }
}