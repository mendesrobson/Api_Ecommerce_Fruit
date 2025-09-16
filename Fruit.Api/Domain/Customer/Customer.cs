namespace Fruit.Api.Domain.Customer;


public class Customer
{
    public int CustId { get; set; }
    public string? Name { get; set; }
    public string? MobileNo { get; set; }
    public string? Password { get; set; }
}


public class LoginRequest
{
    public string? UserName { get; set; }
    public string? UserPassword { get; set; }
}
