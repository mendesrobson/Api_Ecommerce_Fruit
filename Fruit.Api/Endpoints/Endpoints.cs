namespace Fruit.Api.Endpoints;

public static class Endpoints
{
    public static void MapAllEndpoints(this IEndpointRouteBuilder app)
    { 
        app.MapAuthentication();
        app.MapProductEndpoints();
        app.MapCustomerEndpoints();
        app.MapCategoryEndpoints();
    }
}
