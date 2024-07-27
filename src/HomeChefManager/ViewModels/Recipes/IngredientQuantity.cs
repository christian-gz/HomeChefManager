using HomeChefManager.ViewModels.Ingredients;
using ReactiveUI;

namespace HomeChefManager.ViewModels.Recipes;

public class IngredientQuantity : ViewModelBase
{
    private IngredientViewModel _ingredientViewModel;
    private int _quantity;

    public IngredientViewModel IngredientViewModel
    {
        get => _ingredientViewModel;
        set => this.RaiseAndSetIfChanged(ref _ingredientViewModel, value);
    }

    public int Quantity
    {
        get => _quantity;
        set => this.RaiseAndSetIfChanged(ref _quantity, value);
    }

    public IngredientQuantity(IngredientViewModel ingredientViewModel, int quantity)
    {
        _ingredientViewModel = ingredientViewModel;
        _quantity = quantity;
    }
}