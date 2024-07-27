using HomeChefManager.Models;
using HomeChefManager.Services;
using HomeChefManager.ViewModels.Ingredients;
using Moq;

namespace HomeChefManager.Tests;

public class IngredientListViewModelTest
{
    [Fact]
    public void Constructor_InitializesIngredientsFromService()
    {
        // Arrange
        var mockService = new Mock<IIngredientService>();
        mockService.Setup(s => s.GetIngredients()).Returns(new List<Ingredient>
        {
            new Ingredient(1, "Sugar", 100, Unit.Gram),
            new Ingredient(2, "Salt", 200, Unit.Gram)
        });

        // Act
        var viewModel = new IngredientListViewModel(mockService.Object);

        // Assert
        Assert.Equal(2, viewModel.IngredientList.Count);
    }

    [Fact]
    public void AddIngredient_AddsIngredientToList()
    {
        // Arrange
        var mockService = new Mock<IIngredientService>();
        var ingredientToAdd = new Ingredient(0, "Flour", 500, Unit.Gram);
        mockService.Setup(s => s.AddIngredient(It.IsAny<Ingredient>())).Returns(3);

        var viewModel = new IngredientListViewModel(mockService.Object);
        var ingredientViewModel = new IngredientViewModel(ingredientToAdd);

        // Act
        viewModel.AddIngredient(ingredientViewModel);

        // Assert
        Assert.Single(viewModel.IngredientList);
        Assert.Equal(3, viewModel.IngredientList[0].Id);
    }

    [Fact]
    public void RemoveIngredient_RemovesIngredientFromList()
    {
        // Arrange
        var mockService = new Mock<IIngredientService>();
        var ingredientToRemove = new IngredientViewModel(new Ingredient(0, "Water", 1000, Unit.Milliliter));
        var viewModel = new IngredientListViewModel(mockService.Object);

        viewModel.IngredientList.Add(ingredientToRemove);

        // Act
        viewModel.RemoveIngredient(ingredientToRemove);

        // Assert
        Assert.Empty(viewModel.IngredientList);
        mockService.Verify(s => s.RemoveIngredient(It.IsAny<Ingredient>()), Times.Once());
    }

    [Fact]
    public void HandleIngredientQuantityReduced_UpdatesQuantityInList()
    {
        // Arrange
        var mockService = new Mock<IIngredientService>();
        var ingredientViewModel = new IngredientViewModel(new Ingredient(3, "Water", 1000, Unit.Milliliter));
        var viewModel = new IngredientListViewModel(mockService.Object);
        viewModel.IngredientList.Add(ingredientViewModel);

        // Act
        var editedIngredient = new Ingredient(3, "Water", 500, Unit.Milliliter);
        mockService.Raise(m => m.IngredientQuantityReduced += null, new IngredientEventArgs(editedIngredient));

        // Assert
        Assert.Equal(500, viewModel.IngredientList[0].Quantity);
    }
}