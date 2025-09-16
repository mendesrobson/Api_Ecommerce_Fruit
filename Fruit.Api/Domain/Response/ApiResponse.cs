namespace Fruit.Api.Domain.Response;

public class ApiResponse<T>
{
    public string? Message { get; set; }
    public bool Result { get; set; }
    public T? Data { get; set; }
}
