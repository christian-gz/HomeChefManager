using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Avalonia.Controls.Converters;
using HomeChefManager.Services;
using HomeChefManager.Utils;
using HomeChefManager.ViewModels.Ingredients;
using ReactiveUI;

namespace HomeChefManager.ViewModels.Recipes;

public class RecipeListViewModel : ViewModelBase
{
    public RecipeListViewModel()
    {
        RecipeList = new ObservableCollection<RecipeViewModel>
        {
            new RecipeViewModel ("Pizza", new ObservableCollection<IngredientQuantity>())
            {
                Servings = 2,
                TimeToPrepare = 20,
                TimeToCook = 30,
                Directions = "Directions",
                Notes = "Notes"
            },
        };
    }

    public RecipeListViewModel(IRecipeService recipeService, IIngredientService ingredientService, ICookRecipeViewModel cookRecipeViewModel)
    {
        _recipeService = recipeService;
        RecipeList = new ObservableCollection<RecipeViewModel>();

        foreach (var recipe in _recipeService.GetRecipes())
        {
            RecipeList.Add(new RecipeViewModel(recipe));
        }

        _ingredientService = ingredientService;
        _ingredientService.IngredientsEdited += HandleIngredientsEdited;
        IngredientViewModelList = new ObservableCollection<IngredientViewModel>();

        LoadIngredients();

        AddRecipeViewModel = new AddRecipeViewModel();
        AddRecipeViewModel.RecipeAdded += HandleRecipeAdded;
        AddRecipeViewModel.RecipeCanceled += HandleRecipeCanceled;
        AddRecipeViewModel.Ingredients = IngredientViewModelList;

        DetailRecipeViewModel = new DetailRecipeViewModel();
        DetailRecipeViewModel.RecipeCanceled += HandleRecipeCanceled;
        DetailRecipeViewModel.RecipeEdited += HandleRecipeEdited;
        DetailRecipeViewModel.RecipeRemoved += HandleRecipeRemoved;
        DetailRecipeViewModel.Ingredients = IngredientViewModelList;

        CookRecipeViewModel = cookRecipeViewModel;
        CookRecipeViewModel.RecipeCooked += HandleRecipeCooked;
        CookRecipeViewModel.RecipeCanceled += HandleRecipeCanceled;

        OpenAddRecipeCommand = ReactiveCommand.Create(() =>
        {
            IsAddRecipeDialogOpen = true;
        });

        OpenDetailRecipeCommand = ReactiveCommand.Create<RecipeViewModel>(OpenDetailRecipe);
        OpenCookRecipeCommand = ReactiveCommand.Create<RecipeViewModel>(OpenCookRecipe);
    }

    private readonly IRecipeService _recipeService;
    private readonly IIngredientService _ingredientService;
    private bool _isAddRecipeDialogOpen;
    private bool _isDetailRecipeDialogOpen;
    private bool _isCookRecipeDialogOpen;

    public ObservableCollection<RecipeViewModel> RecipeList { get; }
    public ObservableCollection<IngredientViewModel> IngredientViewModelList { get; }
    public AddRecipeViewModel AddRecipeViewModel { get; }
    public DetailRecipeViewModel DetailRecipeViewModel { get; }
    public ICookRecipeViewModel CookRecipeViewModel { get; set; }
    public ICommand OpenAddRecipeCommand { get; }
    public ICommand OpenDetailRecipeCommand { get; }
    public ICommand OpenCookRecipeCommand { get; }

    public bool IsAddRecipeDialogOpen
    {
        get => _isAddRecipeDialogOpen;
        set => this.RaiseAndSetIfChanged(ref _isAddRecipeDialogOpen, value);
    }
    public bool IsDetailRecipeDialogOpen
    {
        get => _isDetailRecipeDialogOpen;
        set => this.RaiseAndSetIfChanged(ref _isDetailRecipeDialogOpen, value);
    }
    public bool IsCookRecipeDialogOpen
    {
        get => _isCookRecipeDialogOpen;
        set => this.RaiseAndSetIfChanged(ref _isCookRecipeDialogOpen, value);
    }

    private void HandleRecipeAdded(object? o, RecipeEventArgs e)
    {
        if (e.RecipeViewModel != null)
        {
            AddRecipe(e.RecipeViewModel);
        }

        IsAddRecipeDialogOpen = false;
    }

    private void HandleRecipeEdited(object? o, RecipeEventArgs e)
    {
        if (e.RecipeViewModel != null)
        {
            UpdateRecipe(e.RecipeViewModel);
        }

        IsDetailRecipeDialogOpen = false;
    }

    private void HandleRecipeRemoved(object? o, RecipeEventArgs e)
    {
        if (e.RecipeViewModel != null)
        {
            RemoveRecipe(e.RecipeViewModel);
        }

        IsDetailRecipeDialogOpen = false;
    }

    private void HandleRecipeCanceled(object? o, RecipeEventArgs e)
    {
        if (e.RecipeViewModel == null)
        {
            IsAddRecipeDialogOpen = false;
            IsDetailRecipeDialogOpen = false;
            IsCookRecipeDialogOpen = false;
        }
    }

    private void HandleRecipeCooked(object? o, RecipeEventArgs e)
    {
        if (e.RecipeViewModel != null)
        {
            foreach (var item in e.RecipeViewModel.Ingredients)
            {
                var ingredient = IngredientViewModelList.FirstOrDefault(i => i.Id == item.IngredientViewModel.Id);

                if (ingredient == null)
                {
                    return;
                }

                ingredient.Quantity = ingredient.Quantity >= item.Quantity ? ingredient.Quantity - item.Quantity : 0;

                _ingredientService.ReduceIngredientQuantity(ingredient.ToIngredient());
            }
        }

        IsCookRecipeDialogOpen = false;
    }

    private void HandleIngredientsEdited(object? o, IngredientEventArgs e)
    {
        if (e.Ingredient != null)
        {
            try
            {
                var ingredient = _ingredientService.GetIngredient(e.Ingredient);

                var ingredientToUpdate = IngredientViewModelList.FirstOrDefault(i => i.Id == ingredient.Id);

                if (ingredientToUpdate == null)
                {
                    // Ingredient in DB but not in List
                    IngredientViewModelList.Add(new IngredientViewModel(ingredient));
                }
                else
                {
                    // Ingredient in DB and in List
                    ingredientToUpdate.Name = ingredient.Name;
                    ingredientToUpdate.Quantity = ingredient.Quantity;
                    ingredientToUpdate.Unit = ingredient.Unit;
                    ingredientToUpdate.Category = ingredient.Category;
                }
            }
            catch (RecordNotFoundException ex)
            {
                // Ingredient not in DB
                var deletedIngredient = IngredientViewModelList.FirstOrDefault(i => i.Id == e.Ingredient.Id);
                if (deletedIngredient != null) IngredientViewModelList.Remove(deletedIngredient);
            }
            catch (Exception ex)
            {
                // Todo: inform user
                Console.WriteLine(ex.Message);
            }

        }
    }

    public void AddRecipe(RecipeViewModel recipeViewModel)
    {
        try
        {
            int id = _recipeService.AddRecipe(recipeViewModel.ToRecipe());
            recipeViewModel.Id = id;
            RecipeList.Add(recipeViewModel);
        }
        catch (Exception e)
        {
            // Todo: inform user
            Console.WriteLine(e);
        }
    }

    public void RemoveRecipe(RecipeViewModel recipeViewModel)
    {
        try
        {
            _recipeService.RemoveRecipe(recipeViewModel.ToRecipe());
            RecipeList.Remove(recipeViewModel);
        }
        catch (Exception e)
        {
            // Todo: inform user
            Console.WriteLine(e);
        }
    }

    private void UpdateRecipe(RecipeViewModel recipeViewModel)
    {
        try
        {
            _recipeService.UpdateRecipe(recipeViewModel.ToRecipe());
        }
        catch (Exception e)
        {
            // Todo: inform user
            Console.WriteLine(e);
        }
    }

    private void OpenDetailRecipe(RecipeViewModel recipeViewModel)
    {
        try
        {
            var recipeDetails = new RecipeViewModel(_recipeService.GetRecipeDetails(recipeViewModel.ToRecipe()));
            recipeViewModel.Ingredients = recipeDetails.Ingredients;
            recipeViewModel.Directions = recipeDetails.Directions;
            recipeViewModel.Notes = recipeDetails.Notes;

            DetailRecipeViewModel.CurrentRecipeViewModel = recipeViewModel;
            IsDetailRecipeDialogOpen = true;
        }
        catch (Exception e)
        {
            // Todo: inform user
            Console.WriteLine(e);
        }
    }

    private void OpenCookRecipe(RecipeViewModel recipeViewModel)
    {
        try
        {
            var recipeDetails = new RecipeViewModel(_recipeService.GetRecipeDetails(recipeViewModel.ToRecipe()));
            recipeViewModel.Ingredients = recipeDetails.Ingredients;
            recipeViewModel.Directions = recipeDetails.Directions;
            recipeViewModel.Notes = recipeDetails.Notes;

            CookRecipeViewModel.CurrentRecipeViewModel = recipeViewModel;
            IsCookRecipeDialogOpen = true;
        }
        catch (Exception e)
        {
            // Todo: inform user
            Console.WriteLine(e);
        }
    }

    private void LoadIngredients()
    {
        IngredientViewModelList.Clear();
        try
        {
            foreach (var ingredient in _ingredientService.GetIngredients())
            {
                IngredientViewModelList.Add(new IngredientViewModel(ingredient));
            }
        }
        catch (Exception e)
        {
            // Todo: inform user
            Console.WriteLine(e);
        }
    }
}