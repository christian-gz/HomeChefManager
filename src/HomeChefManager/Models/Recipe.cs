using System.Collections.Generic;

namespace HomeChefManager.Models;

public class Recipe
{
    public int? Id { get; set; }
    public string Name { get; set; }
    public int? Servings { get; set; }
    public int? TimeToPrepare { get; set; }
    public int? TimeToCook { get; set; }
    public Dictionary<Ingredient, int> Ingredients { get; set; }
    public string? Directions { get; set; }
    public string? Notes { get; set; }

    public Recipe(string name, Dictionary<Ingredient, int> ingredients, int? id = null)
    {
      Id = id;
      Name = name;
      Ingredients = ingredients;
    }

    public override string ToString()
    {
        var recipe = $"ID: {Id}, Name: {Name}, Servings: {Servings}, Ingredients: \n";
        foreach (var ingredient in Ingredients)
        {
            recipe += $"{ingredient.Value} {ingredient.Key.Unit} of {ingredient.Key.Name}\n";
        }
        return recipe;
    }
}