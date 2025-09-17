using Fruit.Api.Domain.Category;
using Fruit.Api.Domain.Response;

namespace Fruit.Api.Endpoints;

public static class CategoryEndpoints
{
    public static void MapCategoryEndpoints(this IEndpointRouteBuilder app)
    {
        // Endpoint: GetAllCategory
        app.MapGet("/api/category/GetAllCategory", () =>
        {
            // Simula a busca de todas as categorias do banco de dados
            var allCategories = new List<Category>
            {
                new Category { CategoryId = 55, CategoryName = "Fruits & Vegetables", ParentCategoryId = 0, UserId = null },
                new Category { CategoryId = 56, CategoryName = "Foodgrains", ParentCategoryId = 0, UserId = null },
                new Category { CategoryId = 57, CategoryName = "Bakery", ParentCategoryId = 0, UserId = null },
                new Category { CategoryId = 58, CategoryName = "Beverages", ParentCategoryId = 0, UserId = null }
            };

            var response = new ApiResponse<List<Category>>
            {
                Message = "",
                Result = true,
                Data = allCategories
            };

            return Results.Ok(response);
        })
        .WithOpenApi();
    }
}
