using System;

namespace HomeChefManager.ViewModels.Ingredients;

public class IngredientViewModelEventArgs : EventArgs
{
    public IngredientViewModel? IngredientViewModel { get; private set; }

    public IngredientViewModelEventArgs(IngredientViewModel? ingredient)
    {
        IngredientViewModel = ingredient;
    }
}