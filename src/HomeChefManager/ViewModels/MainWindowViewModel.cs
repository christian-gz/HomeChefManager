using System.Windows.Input;
using HomeChefManager.ViewModels.Ingredients;
using HomeChefManager.ViewModels.Recipes;
using ReactiveUI;

namespace HomeChefManager.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    public MainWindowViewModel() { }
    public MainWindowViewModel(RecipeListViewModel recipeListViewModel, IngredientListViewModel ingredientListViewModel)
    {
        _recipeListViewModel = recipeListViewModel;
        _ingredientViewModel = ingredientListViewModel;

        _currentViewModel = _recipeListViewModel;

        NavigateRecipesCommand = ReactiveCommand.Create(NavigateRecipes);
        NavigateIngredientsCommand = ReactiveCommand.Create(NavigateIngredients);
    }

    private ViewModelBase _currentViewModel;
    private readonly ViewModelBase _recipeListViewModel;
    private readonly ViewModelBase _ingredientViewModel;

    public ICommand NavigateRecipesCommand { get; set; }
    public ICommand NavigateIngredientsCommand { get; set; }

    public ViewModelBase CurrentViewModel
    {
        get => _currentViewModel;
        private set => this.RaiseAndSetIfChanged(ref _currentViewModel, value);
    }

    private void NavigateRecipes()
    {
        CurrentViewModel = _recipeListViewModel;
    }

    private void NavigateIngredients()
    {
        CurrentViewModel = _ingredientViewModel;
        ((RecipeListViewModel)_recipeListViewModel).IsCookRecipeDialogOpen = false;
    }
}