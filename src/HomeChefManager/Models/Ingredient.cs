namespace HomeChefManager.Models;

public class Ingredient
{
    public int? Id { get; set; }
    public string Name { get; set; }
    public int Quantity { get; set; }
    public Unit Unit { get; set; }
    public string? Category { get; set; }

    public Ingredient(int? id, string name, int quantity, Unit unit)
    {
        Id = id;
        Name = name;
        Quantity = quantity;
        Unit = unit;
    }

    public override string ToString()
    {
        return $"{ Name } ({ Unit })";
    }
}