using System.Collections.ObjectModel;
using HomeChefManager.Models;
using HomeChefManager.Services;
using HomeChefManager.Utils;
using HomeChefManager.ViewModels.Ingredients;
using HomeChefManager.ViewModels.Recipes;
using Moq;

namespace HomeChefManager.Tests;

public class RecipeListViewModelTest
{
    [Fact]
    public void Constructor_InitializesRecipeFromService()
    {
        // Arrange
        var mockRecipeService = new Mock<IRecipeService>();
        var mockIngredientService = new Mock<IIngredientService>();
        var mockCookRecipeViewModel = new Mock<ICookRecipeViewModel>();

        var ingredients = new List<Ingredient>
        {
            new Ingredient(1, "Sugar", 100, Unit.Gram)
        };
        mockIngredientService.Setup(s => s.GetIngredients()).Returns(ingredients);

        var recipes = new List<Recipe>
        {
            new Recipe("Ice", new Dictionary<Ingredient, int>(), 1){
                Servings = 2,
                TimeToPrepare = 10,
                TimeToCook = 20,
                Directions = "Directions",
                Notes = "Notes"
            }
        };
        mockRecipeService.Setup(s => s.GetRecipes()).Returns(recipes);

        // Act
        var viewModel = new RecipeListViewModel( mockRecipeService.Object, mockIngredientService.Object, mockCookRecipeViewModel.Object);
        var viewModelIngredients = viewModel.IngredientViewModelList;
        var viewModelRecipes = viewModel.RecipeList;

        // Assert
        Assert.Single(viewModel.IngredientViewModelList);
        Assert.Single(viewModel.RecipeList);

        var viewModelIngredient = viewModelIngredients.First();
        Assert.Equal(1, viewModelIngredient.Id);
        Assert.Equal("Sugar", viewModelIngredient.Name);
        Assert.Equal(100, viewModelIngredient.Quantity);
        Assert.Equal(Unit.Gram, viewModelIngredient.Unit);

        var viewModelRecipe = viewModelRecipes.First();
        Assert.Equal(1, viewModelRecipe.Id);
        Assert.Equal("Ice", viewModelRecipe.Name);
        Assert.Equal(2, viewModelRecipe.Servings);
        Assert.Equal(10, viewModelRecipe.TimeToPrepare);
        Assert.Equal(20, viewModelRecipe.TimeToCook);
        Assert.Equal("Directions", viewModelRecipe.Directions);
        Assert.Equal("Notes", viewModelRecipe.Notes);
    }

    [Fact]
    public void AddRecipe_AddsRecipeToList()
    {
        // Arrange
        var mockRecipeService = new Mock<IRecipeService>();
        var mockIngredientService = new Mock<IIngredientService>();
        var mockCookRecipeViewModel = new Mock<ICookRecipeViewModel>();
        var recipeToAdd = new Recipe("Ice", new Dictionary<Ingredient, int>(), 1)
        {
            Servings = 2,
            TimeToPrepare = 10,
            TimeToCook = 20,
            Directions = "Directions",
            Notes = "Notes"
        };

        mockRecipeService.Setup(s => s.AddRecipe(It.IsAny<Recipe>())).Returns(3);

        var viewModel = new RecipeListViewModel( mockRecipeService.Object, mockIngredientService.Object, mockCookRecipeViewModel.Object);
        var recipeViewModel = new RecipeViewModel(recipeToAdd);

        // Act
        viewModel.AddRecipe(recipeViewModel);

        // Assert
        Assert.Single(viewModel.RecipeList);
        Assert.Equal(3, viewModel.RecipeList[0].Id);
    }

    [Fact]
    public void RemoveRecipe_RemovesRecipeFromList()
    {
        // Arrange
        var mockRecipeService = new Mock<IRecipeService>();
        var mockIngredientService = new Mock<IIngredientService>();
        var mockCookRecipeViewModel = new Mock<ICookRecipeViewModel>();
        var recipeToRemove = new RecipeViewModel("Ice", new ObservableCollection<IngredientQuantity>(), 1)
        {
            Servings = 2,
            TimeToPrepare = 10,
            TimeToCook = 20,
            Directions = "Directions",
            Notes = "Notes"
        };

        var viewModel = new RecipeListViewModel(mockRecipeService.Object, mockIngredientService.Object, mockCookRecipeViewModel.Object);
        viewModel.RecipeList.Add(recipeToRemove);

        // Act
        viewModel.RemoveRecipe(recipeToRemove);

        // Assert
        Assert.Empty(viewModel.RecipeList);
        mockRecipeService.Verify(s => s.RemoveRecipe(It.IsAny<Recipe>()), Times.Once());
    }

    [Fact]
    public void HandleRecipeCooked_UpdatesQuantityInList()
    {
        // Arrange
        var mockRecipeService = new Mock<IRecipeService>();
        var mockIngredientService = new Mock<IIngredientService>();
        var mockCookRecipeViewModel = new Mock<ICookRecipeViewModel>();
        var viewModel = new RecipeListViewModel(mockRecipeService.Object, mockIngredientService.Object, mockCookRecipeViewModel.Object);

        var recipeViewModel = new RecipeViewModel("Ice", new ObservableCollection<IngredientQuantity>(), 1)
        {
            Servings = 2,
            TimeToPrepare = 10,
            TimeToCook = 20,
            Directions = "Directions",
            Notes = "Notes"
        };

        var ingredientViewModel1 = new IngredientViewModel(new Ingredient(1, "Milk", 200, Unit.Milliliter));
        var ingredientViewModel2 = new IngredientViewModel(new Ingredient(2, "Chocolate", 50, Unit.Gram));

        recipeViewModel.Ingredients.Add(new IngredientQuantity(ingredientViewModel1, 100));
        recipeViewModel.Ingredients.Add(new IngredientQuantity(ingredientViewModel2, 25));

        viewModel.RecipeList.Add(recipeViewModel);
        viewModel.IngredientViewModelList.Add(ingredientViewModel1);
        viewModel.IngredientViewModelList.Add(ingredientViewModel2);

        // Act
        mockCookRecipeViewModel.Raise(m => m.RecipeCooked += null, new RecipeEventArgs(recipeViewModel));

        // Assert
        Assert.Equal(1, viewModel.IngredientViewModelList[0].Id);
        Assert.Equal("Milk", viewModel.IngredientViewModelList[0].Name);
        Assert.Equal(100, viewModel.IngredientViewModelList[0].Quantity);
        Assert.Equal(Unit.Milliliter, viewModel.IngredientViewModelList[0].Unit);
        Assert.Null(viewModel.IngredientViewModelList[0].Category);

        Assert.Equal(2, viewModel.IngredientViewModelList[1].Id);
        Assert.Equal("Chocolate", viewModel.IngredientViewModelList[1].Name);
        Assert.Equal(25, viewModel.IngredientViewModelList[1].Quantity);
        Assert.Equal(Unit.Gram, viewModel.IngredientViewModelList[1].Unit);
        Assert.Null(viewModel.IngredientViewModelList[1].Category);
    }

    [Fact]
    public void HandleIngredientsEdited_WhenIngredientQuantityChanges_UpdatesIngredientInList()
    {
        // Arrange
        var mockRecipeService = new Mock<IRecipeService>();
        var mockIngredientService = new Mock<IIngredientService>();
        var mockCookRecipeViewModel = new Mock<ICookRecipeViewModel>();
        var viewModel = new RecipeListViewModel(mockRecipeService.Object, mockIngredientService.Object, mockCookRecipeViewModel.Object);

        var ingredientViewModel1 = new IngredientViewModel(new Ingredient(1, "Milk", 200, Unit.Milliliter));
        var ingredientViewModel2 = new IngredientViewModel(new Ingredient(2, "Chocolate", 50, Unit.Gram));

        viewModel.IngredientViewModelList.Add(ingredientViewModel1);
        viewModel.IngredientViewModelList.Add(ingredientViewModel2);

        var ingredientEdited = new Ingredient(1, "Milk", 1000, Unit.Milliliter);
        mockIngredientService
            .Setup(s => s.GetIngredient(It.Is<Ingredient>(i => i.Id == ingredientEdited.Id)))
            .Returns(ingredientEdited);

        // Act
        mockIngredientService.Raise(m => m.IngredientsEdited += null, new IngredientEventArgs(ingredientEdited));

        // Assert
        Assert.Equal(1, viewModel.IngredientViewModelList[0].Id);
        Assert.Equal("Milk", viewModel.IngredientViewModelList[0].Name);
        Assert.Equal(1000, viewModel.IngredientViewModelList[0].Quantity);
        Assert.Equal(Unit.Milliliter, viewModel.IngredientViewModelList[0].Unit);
        Assert.Null(viewModel.IngredientViewModelList[0].Category);

        Assert.Equal(2, viewModel.IngredientViewModelList[1].Id);
        Assert.Equal("Chocolate", viewModel.IngredientViewModelList[1].Name);
        Assert.Equal(50, viewModel.IngredientViewModelList[1].Quantity);
        Assert.Equal(Unit.Gram, viewModel.IngredientViewModelList[1].Unit);
        Assert.Null(viewModel.IngredientViewModelList[1].Category);

        Assert.Equal(2, viewModel.IngredientViewModelList.Count);
    }

    [Fact]
    public void HandleIngredientsEdited_WhenIngredientAdded_UpdatesIngredientList()
    {
        // Arrange
        var mockRecipeService = new Mock<IRecipeService>();
        var mockIngredientService = new Mock<IIngredientService>();
        var mockCookRecipeViewModel = new Mock<ICookRecipeViewModel>();
        var viewModel = new RecipeListViewModel(mockRecipeService.Object, mockIngredientService.Object, mockCookRecipeViewModel.Object);

        var ingredientViewModel = new IngredientViewModel(new Ingredient(1, "Milk", 200, Unit.Milliliter));

        viewModel.IngredientViewModelList.Add(ingredientViewModel);

        var ingredientAdded = new Ingredient(2, "Chocolate", 50, Unit.Gram);
        mockIngredientService
            .Setup(s => s.GetIngredient(It.Is<Ingredient>(i => i.Id == ingredientAdded.Id)))
            .Returns(ingredientAdded);

        // Act
        mockIngredientService.Raise(m => m.IngredientsEdited += null, new IngredientEventArgs(ingredientAdded));

        // Assert
        Assert.Equal(1, viewModel.IngredientViewModelList[0].Id);
        Assert.Equal("Milk", viewModel.IngredientViewModelList[0].Name);
        Assert.Equal(200, viewModel.IngredientViewModelList[0].Quantity);
        Assert.Equal(Unit.Milliliter, viewModel.IngredientViewModelList[0].Unit);
        Assert.Null(viewModel.IngredientViewModelList[0].Category);

        Assert.Equal(ingredientAdded.Id, viewModel.IngredientViewModelList[1].Id);
        Assert.Equal(ingredientAdded.Name, viewModel.IngredientViewModelList[1].Name);
        Assert.Equal(ingredientAdded.Quantity, viewModel.IngredientViewModelList[1].Quantity);
        Assert.Equal(ingredientAdded.Unit, viewModel.IngredientViewModelList[1].Unit);
        Assert.Equal(ingredientAdded.Category, viewModel.IngredientViewModelList[1].Category);

        Assert.Equal(2, viewModel.IngredientViewModelList.Count);
    }

    [Fact]
    public void HandleIngredientsEdited_WhenIngredientRemoved_UpdatesIngredientList()
    {
        // Arrange
        var mockRecipeService = new Mock<IRecipeService>();
        var mockIngredientService = new Mock<IIngredientService>();
        var mockCookRecipeViewModel = new Mock<ICookRecipeViewModel>();
        var viewModel = new RecipeListViewModel(mockRecipeService.Object, mockIngredientService.Object, mockCookRecipeViewModel.Object);

        var ingredientViewModel1 = new IngredientViewModel(new Ingredient(1, "Milk", 200, Unit.Milliliter));
        var ingredientViewModel2 = new IngredientViewModel(new Ingredient(2, "Chocolate", 50, Unit.Gram));

        viewModel.IngredientViewModelList.Add(ingredientViewModel1);
        viewModel.IngredientViewModelList.Add(ingredientViewModel2);

        var ingredientRemoved = new Ingredient(2, "Chocolate", 50, Unit.Gram);
        mockIngredientService
            .Setup(s => s.GetIngredient(It.Is<Ingredient>(i => i.Id == ingredientRemoved.Id)))
            .Throws(new RecordNotFoundException(string.Empty));

        // Act
        mockIngredientService.Raise(m => m.IngredientsEdited += null, new IngredientEventArgs(ingredientRemoved));

        // Assert
        Assert.Equal(1, viewModel.IngredientViewModelList[0].Id);
        Assert.Equal("Milk", viewModel.IngredientViewModelList[0].Name);
        Assert.Equal(200, viewModel.IngredientViewModelList[0].Quantity);
        Assert.Equal(Unit.Milliliter, viewModel.IngredientViewModelList[0].Unit);
        Assert.Null(viewModel.IngredientViewModelList[0].Category);

        Assert.Single(viewModel.IngredientViewModelList);
        Assert.DoesNotContain(viewModel.IngredientViewModelList, i => i.Id == ingredientRemoved.Id);
        Assert.Contains(viewModel.IngredientViewModelList, i => i.Id == ingredientViewModel1.Id);
    }
}