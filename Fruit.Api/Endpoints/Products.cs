using Fruit.Api.Domain.Product;

namespace Fruit.Api.Endpoints;

public static class Products
{
    public static void MapProductEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/ecommerce/CreateProduct", (Product product, ILogger<Program> logger) =>
        {
            logger.LogInformation($"Produto '{product.ProductName}' recebido.");

            return Results.Created($"/api/BigBasket/{product.ProductId}", product);
        })
        .WithOpenApi()
        .RequireAuthorization();
    }
}
