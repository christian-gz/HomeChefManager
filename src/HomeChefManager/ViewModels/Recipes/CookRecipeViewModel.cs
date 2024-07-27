using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using HomeChefManager.Models;
using HomeChefManager.ViewModels.Ingredients;
using ReactiveUI;

namespace HomeChefManager.ViewModels.Recipes;

public class CookRecipeViewModel : ViewModelBase, ICookRecipeViewModel
{
    public CookRecipeViewModel()
    {
        var ingredientsComplete =
            this.WhenAnyValue(c => c.EnoughIngredients, e => !!e);
        var ingredientsMissing =
            this.WhenAnyValue(c => c.EnoughIngredients, e => !e);

        FinishedCookingCommand = ReactiveCommand.Create(FinishedCooking, ingredientsComplete);
        FinishedCookingIngredientsMissingCommand = ReactiveCommand.Create(FinishedCookingIngredientsMissing, ingredientsMissing);
        CancelCommand = ReactiveCommand.Create(Cancel);

        MissingIngredients = new ObservableCollection<string>();

        CurrentRecipeViewModel = new RecipeViewModel("Ice Cream", new ObservableCollection<IngredientQuantity>())
        {
            Servings = 1,
            TimeToPrepare = 10,
            TimeToCook = 20,
            Directions = "Directions",
            Notes = "Notes",
            Ingredients = new ObservableCollection<IngredientQuantity>
            {
                new (new IngredientViewModel("Milk", 1000, Unit.Milliliter), 300),
                new (new IngredientViewModel("Banana", 1000, Unit.Gram), 100),
                new (new IngredientViewModel("Chocolate", 1000, Unit.Gram), 50)
            }
        };
    }

    private RecipeViewModel? _currentRecipeViewModel;
    private bool _enoughIngredients;

    public event EventHandler<RecipeEventArgs>? RecipeCooked;
    public event EventHandler<RecipeEventArgs>? RecipeCanceled;
    public ICommand FinishedCookingCommand { get; }
    public ICommand FinishedCookingIngredientsMissingCommand { get; }
    public ICommand CancelCommand { get; }

    public ObservableCollection<string> MissingIngredients { get; }

    public bool EnoughIngredients
    {
        get => _enoughIngredients;
        set => this.RaiseAndSetIfChanged(ref _enoughIngredients, value);
    }

    public RecipeViewModel? CurrentRecipeViewModel
    {
        get => _currentRecipeViewModel;
        set
        {
            this.RaiseAndSetIfChanged(ref _currentRecipeViewModel, value);
            MissingIngredients.Clear();
            CheckIngredientQuantity();
        }
    }

    private void FinishedCooking()
    {
        RecipeCooked?.Invoke(this, new RecipeEventArgs(CurrentRecipeViewModel));
    }

    private void FinishedCookingIngredientsMissing()
    {
        RecipeCooked?.Invoke(this, new RecipeEventArgs(CurrentRecipeViewModel));
    }

    private void Cancel()
    {
        RecipeCanceled?.Invoke(this, new RecipeEventArgs(null));
    }

    private void CheckIngredientQuantity()
    {
        if (CurrentRecipeViewModel == null)
        {
            return;
        }
        
        EnoughIngredients = true;

        foreach (var item in CurrentRecipeViewModel.Ingredients)
        {
            var difference = item.IngredientViewModel.Quantity - item.Quantity;
            if (difference < 0)
            {
                EnoughIngredients = false;
                MissingIngredients.Add($"{ item.IngredientViewModel.Name } { int.Abs(difference) } { item.IngredientViewModel.Unit }");
            }
        }
    }
}