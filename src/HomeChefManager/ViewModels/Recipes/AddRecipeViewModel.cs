using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using ReactiveUI;

namespace HomeChefManager.ViewModels.Recipes;

public class AddRecipeViewModel : RecipeDialogBaseViewModel
{
    public AddRecipeViewModel()
    {
        AddRecipeCommand = ReactiveCommand.Create(AddRecipe, NoErrors);
    }
    
    public event EventHandler<RecipeEventArgs>? RecipeAdded;
    public ICommand AddRecipeCommand { get; }
    private void AddRecipe()
    {
        ValidateAll();

        if (HasErrors)
        {
            return;
        }

        var recipeViewModel = new RecipeViewModel(Name, new ObservableCollection<IngredientQuantity>(IngredientSelectionToIngredientQuantity(IngredientStringQuantities)))
        {
            Servings = string.IsNullOrEmpty(Servings) ? null : int.Parse(Servings),
            TimeToPrepare = string.IsNullOrEmpty(TimeToPrepare) ? null : int.Parse(TimeToPrepare),
            TimeToCook = string.IsNullOrEmpty(TimeToCook) ? null : int.Parse(TimeToCook),
            Directions = Directions,
            Notes = Notes,
        };

        ClearDialog();

        RecipeAdded?.Invoke(this, new RecipeEventArgs(recipeViewModel));
    }

}
