using System;
using HomeChefManager.Models;

namespace HomeChefManager.Services;

public class IngredientEventArgs : EventArgs
{
    public Ingredient? Ingredient { get; }

    public IngredientEventArgs(Ingredient? ingredient)
    {
        Ingredient = ingredient;
    }
}