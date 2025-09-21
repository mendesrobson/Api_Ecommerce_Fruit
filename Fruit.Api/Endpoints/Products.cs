using Fruit.Api.Domain.Product;
using Fruit.Api.Domain.Response;

namespace Fruit.Api.Endpoints;

public static class Products
{
    public static void MapProductEndpoints(this IEndpointRouteBuilder app)
    {
        // 1. Endpoint CreateProduct
        app.MapPost("/api/ecommerce/CreateProduct", (Product product, ILogger<Program> logger) =>
        {
            logger.LogInformation($"Produto '{product.ProductName}' recebido.");

            return Results.Created($"/api/BigBasket/{product.ProductId}", product);
        })
        .WithOpenApi();
        //.RequireAuthorization();

        // 2. Endpoint GetAllProducts
        app.MapGet("/api/ecommerce/GetAllProducts", () =>
        {
            var allProducts = new List<ProductsResponse>
            {
                new ProductsResponse
                {
                    ProductId = 2857,
                    ProductSku = "NNG",
                    ProductName = "Watchsssqwertzu",
                    ProductPrice = 998,
                    ProductShortName = "Watch1 edit",
                    ProductDescription = "Watchnjhuyu",
                    CreatedDate = DateTime.Parse("2024-12-05T14:36:17.93"),
                    DeliveryTimeSpan = "1-2 Days",
                    CategoryId = 67,
                    ProductImageUrl = "https://images.unsplash.com/photo-1524805444758-089113d48a6d?w=500&auto=format&fit=crop&q=60&ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxzZWFyY2h8Mnx8d2F0Y2hzfGVufDB8fDB8fHww",
                    CategoryName = "Edible Oils"
                }
            };

            var response = new ApiResponse<List<ProductsResponse>>
            {
                Message = "",
                Result = true,
                Data = allProducts
            };

            return Results.Ok(response);
        })
        .WithOpenApi();
        //.RequireAuthorization();

        // 3. Endpoint GetAllProductsByCategoryId
        app.MapGet("/api/ecommerce/GetAllProductsByCategoryId", (int id) =>
        {
            var productsByCategoryId = new List<ProductsResponse>();
            if (id == 67)
            {
                productsByCategoryId.Add(new ProductsResponse
                {
                    ProductId = 2857,
                    ProductSku = "NNG",
                    ProductName = "Watchsssqwertzu",
                    ProductPrice = 998,
                    ProductShortName = "Watch1 edit",
                    ProductDescription = "Watchnjhuyu",
                    CreatedDate = DateTime.Parse("2024-12-05T14:36:17.93"),
                    DeliveryTimeSpan = "1-2 Days",
                    CategoryId = 67,
                    ProductImageUrl = "https://images.unsplash.com/photo-1524805444758-089113d48a6d?w=500&auto=format&fit=crop&q=60&ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxzZWFyY2h8Mnx8d2F0Y2hzfGVufDB8fDB8fHww",
                    CategoryName = "Edible Oils"
                });
            }

            var response = new ApiResponse<List<ProductsResponse>>
            {
                Message = "",
                Result = true,
                Data = productsByCategoryId
            };

            return Results.Ok(response);
        })
        .WithOpenApi();
        //.RequireAuthorization();
    }
}
