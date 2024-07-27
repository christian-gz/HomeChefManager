namespace HomeChefManager.Models;

public class RecipeDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int? Servings { get; set; }
    public int? TimeToPrepare { get; set; }
    public int? TimeToCook { get; set; }
    public string? Directions { get; set; }
    public string? Notes { get; set; }
}