using System;
using System.Windows.Input;
using ReactiveUI;

namespace HomeChefManager.ViewModels.Ingredients;

public class DetailIngredientViewModel : IngredientDialogBaseViewModel
{
    public DetailIngredientViewModel()
    {
        ConfirmEditIngredientCommand = ReactiveCommand.Create(ConfirmEditIngredient, NoErrors);
        RemoveIngredientCommand = ReactiveCommand.Create(RemoveIngredient);
    }

    private IngredientViewModel? _currentIngredientViewModel;

    public event EventHandler<IngredientViewModelEventArgs>? IngredientEdited;
    public event EventHandler<IngredientViewModelEventArgs>? IngredientRemoved;
    public ICommand ConfirmEditIngredientCommand { get; }
    public ICommand RemoveIngredientCommand { get; }

    public IngredientViewModel? CurrentIngredientViewModel
    {
        get => _currentIngredientViewModel;
        set
        {
            if (value != null)
            {
                Name = value.Name;
                Quantity = value.Quantity.ToString();
                Unit = value.Unit;
                Category = value.Category;

                _currentIngredientViewModel = value;
            }
        }
    }

    private void ConfirmEditIngredient()
    {
        if (CurrentIngredientViewModel == null)
        {
            return;
        }

        ValidateAll();

        if (HasErrors)
            return;

        CurrentIngredientViewModel.Name = Name;
        CurrentIngredientViewModel.Quantity = int.Parse(Quantity);
        CurrentIngredientViewModel.Unit = Unit ?? Models.Unit.Gram;
        CurrentIngredientViewModel.Category = Category;

        IngredientEdited?.Invoke(this, new IngredientViewModelEventArgs(CurrentIngredientViewModel));
    }

    private void RemoveIngredient()
    {
        IngredientRemoved?.Invoke(this, new IngredientViewModelEventArgs(CurrentIngredientViewModel));
    }
}