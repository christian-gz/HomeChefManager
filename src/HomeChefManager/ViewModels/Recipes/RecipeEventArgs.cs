using System;

namespace HomeChefManager.ViewModels.Recipes;

public class RecipeEventArgs : EventArgs
{
    public RecipeViewModel? RecipeViewModel{ get; private set; }

    public RecipeEventArgs(RecipeViewModel? recipe)
    {
        RecipeViewModel = recipe;
    }
}