namespace HomeChefManager.Models;

public class IngredientDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public string Unit { get; set; } = Models.Unit.Gram.ToString();
    public string? Category { get; set; }
}