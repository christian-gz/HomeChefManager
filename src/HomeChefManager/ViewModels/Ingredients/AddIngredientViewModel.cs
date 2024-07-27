using System;
using System.Windows.Input;
using ReactiveUI;

namespace HomeChefManager.ViewModels.Ingredients;

public class AddIngredientViewModel : IngredientDialogBaseViewModel
{
    public AddIngredientViewModel()
    {
        AddIngredientCommand = ReactiveCommand.Create(AddIngredient, NoErrors);
    }

    public event EventHandler<IngredientViewModelEventArgs>? IngredientAdded;
    public ICommand AddIngredientCommand { get; }

    private void AddIngredient()
    {
        ValidateAll();

        if (HasErrors)
        {
            return;
        }

        var ingredientViewModel = new IngredientViewModel(Name, int.Parse(Quantity), Unit, Category);

        Name = "";
        Quantity = "";
        Unit = null;
        Category = null;
        ClearErrors();

        IngredientAdded?.Invoke(this, new IngredientViewModelEventArgs(ingredientViewModel));
    }
}