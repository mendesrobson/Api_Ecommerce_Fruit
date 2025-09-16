using Fruit.Api.Domain.Customer;
using Fruit.Api.Domain.Response;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Fruit.Api.Endpoints;

public static class CustomerEndpoints
{
    public static void MapCustomerEndpoints(this IEndpointRouteBuilder app)
    {
        // Endpoint: GetAllCustomers
        app.MapGet("/api/customer/GetAllCustomers", () =>
        {
            var customers = new List<Customer>
            {
                new Customer
                {
                    CustId = 2558,
                    Name = "Admin",
                    MobileNo = "amin",
                    Password = "admin"
                }
            };

            var response = new ApiResponse<List<Customer>>
            {
                Message = "",
                Result = true,
                Data = customers
            };

            return Results.Ok(response);
        })
        .WithOpenApi();

        // Endpoint: GetCustomerById
        app.MapGet("/api/customer/GetCustomerById", (int id) =>
        {
            var customer = new Customer
            {
                CustId = 2559,
                Name = "Admin",
                MobileNo = "admin",
                Password = "admin"
            };

            var response = new ApiResponse<Customer>
            {
                Message = "",
                Result = true,
                Data = id == 2559 ? customer : null
            };

            return Results.Ok(response);
        })
        .WithOpenApi();

        // Endpoint: RegisterCustomer
        app.MapPost("/api/customer/RegisterCustomer", (Customer customer, ILogger<Customer> logger) =>
        {
            logger.LogInformation($"Cliente '{customer.Name}' registrado.");

            customer.CustId = 123;

            var response = new ApiResponse<Customer>
            {
                Message = "Registration successful!",
                Result = true,
                Data = customer
            };

            return Results.Ok(response);
        })
        .WithOpenApi();

        // Endpoint: UpdateProfile
        app.MapPost("/api/customer/UpdateProfile", (Customer customer, ILogger<Customer> logger) =>
        {
            logger.LogInformation($"Perfil do cliente '{customer.Name}' atualizado.");

            var response = new ApiResponse<Customer>
            {
                Message = "Profile updated successfully.",
                Result = true,
                Data = customer
            };

            return Results.Ok(response);
        })
        .WithOpenApi()
        .RequireAuthorization();

        // Endpoint: Login
        app.MapPost("/api/customer/login", (LoginRequest loginRequest, IConfiguration config) =>
        {
            if (loginRequest.UserName == "admin" && loginRequest.UserPassword == "admin")
            {
                var accessToken = AuthenticationEndpoints.GenerateJwtToken(loginRequest.UserName, config);
                var refreshToken = Guid.NewGuid().ToString();
                AuthenticationEndpoints.AddRefreshToken(refreshToken, loginRequest.UserName);

                var customerData = new Customer
                {
                    CustId = 2559,
                    Name = "Admin",
                    MobileNo = "admin",
                    Password = "admin"
                };

                var response = new ApiResponse<object>
                {
                    Message = "Login Successful",
                    Result = true,
                    Data = new { accessToken, refreshToken, customer = customerData }
                };

                return Results.Ok(response);
            }

            var errorResponse = new ApiResponse<object>
            {
                Message = "Wrong Mobile No or Password",
                Result = false,
                Data = null
            };

            return Results.BadRequest(errorResponse);
        }).WithOpenApi();
    }
}