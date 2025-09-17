namespace Fruit.Api.Domain.Category;

public record class Category
{
    public int CategoryId { get; set; }
    public string? CategoryName { get; set; }
    public int ParentCategoryId { get; set; }
    public int? UserId { get; set; }
}
