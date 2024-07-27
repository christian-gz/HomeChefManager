using System;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace HomeChefManager.ViewModels.Recipes;

public interface ICookRecipeViewModel
{
    event EventHandler<RecipeEventArgs>? RecipeCooked;
    event EventHandler<RecipeEventArgs>? RecipeCanceled;
    ICommand FinishedCookingCommand { get; }
    ICommand FinishedCookingIngredientsMissingCommand { get; }
    ICommand CancelCommand { get; }
    ObservableCollection<string> MissingIngredients { get; }
    bool EnoughIngredients { get; set; }
    RecipeViewModel? CurrentRecipeViewModel { get; set; }
}