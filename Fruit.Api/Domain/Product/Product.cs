namespace Fruit.Api.Domain.Product;

public record class Product
{
    public int ProductId { get; set; }
    public string ProductSku { get; set; }
    public string ProductName { get; set; }
    public decimal ProductPrice { get; set; }
    public string ProductShortName { get; set; }
    public string ProductDescription { get; set; }
    public DateTime CreatedDate { get; set; }
    public string DeliveryTimeSpan { get; set; }
    public int CategoryId { get; set; }
    public string ProductImageUrl { get; set; }
    public int UserId { get; set; }
}

public record class ProductsResponse
{
    public int ProductId { get; set; }
    public string? ProductSku { get; set; }
    public string? ProductName { get; set; }
    public decimal ProductPrice { get; set; }
    public string? ProductShortName { get; set; }
    public string? ProductDescription { get; set; }
    public DateTime CreatedDate { get; set; }
    public string? DeliveryTimeSpan { get; set; }
    public int CategoryId { get; set; }
    public string? ProductImageUrl { get; set; }
    public string? CategoryName { get; set; } 
}
