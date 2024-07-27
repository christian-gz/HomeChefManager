using System;

namespace HomeChefManager.ViewModels.Recipes;

public class IngredientStringQuantity
{
    private string? _selectedIngredient;

    public event Action? SelectedIngredientChanged;
    public string? SelectedIngredient
    {
        get => _selectedIngredient;
        set
        {
            if (_selectedIngredient != value)
            {
                _selectedIngredient = value;
                SelectedIngredientChanged?.Invoke();
            }
        }
    }

    public int? Quantity { get; set; }
}