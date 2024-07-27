using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using ReactiveUI;

namespace HomeChefManager.ViewModels.Recipes;

public class DetailRecipeViewModel : RecipeDialogBaseViewModel
{
    public DetailRecipeViewModel()
    {
        ConfirmEditRecipeCommand = ReactiveCommand.Create(ConfirmEditRecipe);
        RemoveRecipeCommand = ReactiveCommand.Create(RemoveRecipe);
    }

    private RecipeViewModel? _currentRecipeViewModel;

    public event EventHandler<RecipeEventArgs>? RecipeEdited;
    public event EventHandler<RecipeEventArgs>? RecipeRemoved;
    public ICommand ConfirmEditRecipeCommand { get; }
    public ICommand RemoveRecipeCommand { get; }

    public RecipeViewModel? CurrentRecipeViewModel
    {
        get => _currentRecipeViewModel;
        set
        {
            if (value != null)
            {
                Name = value.Name;
                Servings = value.Servings.ToString();
                TimeToPrepare = value.TimeToPrepare.ToString();
                TimeToCook = value.TimeToCook.ToString();
                Directions = value.Directions;
                Notes = value.Notes;

                foreach (var item in value.Ingredients)
                {
                    IngredientStringQuantities.Add(new IngredientStringQuantity()
                    {
                        SelectedIngredient = item.IngredientViewModel.ToString(),
                        Quantity = item.Quantity
                    });

                }

                _currentRecipeViewModel = value;
            }
        }
    }

    private void ConfirmEditRecipe()
    {
        if (CurrentRecipeViewModel == null)
        {
            return;
        }

        ValidateAll();

        if (HasErrors)
        {
            return;
        }

        CurrentRecipeViewModel.Name = Name;
        CurrentRecipeViewModel.Servings = string.IsNullOrEmpty(Servings) ? null : int.Parse(Servings);
        CurrentRecipeViewModel.TimeToPrepare = string.IsNullOrEmpty(TimeToPrepare) ? null : int.Parse(TimeToPrepare);
        CurrentRecipeViewModel.TimeToCook = string.IsNullOrEmpty(TimeToCook) ? null : int.Parse(TimeToCook);
        CurrentRecipeViewModel.Directions = Directions;
        CurrentRecipeViewModel.Notes = Notes;
        CurrentRecipeViewModel.Ingredients =
            new ObservableCollection<IngredientQuantity>(IngredientSelectionToIngredientQuantity(IngredientStringQuantities));

        ClearDialog();

        RecipeEdited?.Invoke(this, new RecipeEventArgs(CurrentRecipeViewModel));
    }

    private void RemoveRecipe()
    {
        ClearDialog();

        RecipeRemoved?.Invoke(this, new RecipeEventArgs(CurrentRecipeViewModel));
    }
}