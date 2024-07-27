using System;
using System.Collections.Generic;
using System.Linq;
using HomeChefManager.DataAccess;
using HomeChefManager.Models;

namespace HomeChefManager.Services;

public class RecipeService : IRecipeService
{
    public RecipeService(IRecipeRepository recipeRepository)
    {
        _recipeRepository = recipeRepository;
        _recipeBook = new RecipeBook();

        LoadDb();
    }

    private readonly RecipeBook _recipeBook;
    private readonly IRecipeRepository _recipeRepository;

    public IEnumerable<Recipe> GetRecipes()
    {
        return _recipeBook.Recipes;
    }
    
    private void LoadDb()
    {
        try
        {
            _recipeBook.Recipes = _recipeRepository.LoadRecipes();
        }
        catch (Exception e)
        {
            // Todo: inform user the recipes could not get loaded
            Console.WriteLine(e.Message);
        }
    }

    public Recipe GetRecipeDetails(Recipe recipe)
    {
        return _recipeRepository.GetRecipeDetails(recipe);
    }

    public int AddRecipe(Recipe recipe)
    {
        var id = _recipeRepository.AddRecipe(recipe);
        _recipeBook.Recipes.Add(recipe);

        return id;
    }

    public void UpdateRecipe(Recipe recipe)
    {
        _recipeRepository.UpdateRecipe(recipe);

        var itemToUpdate =
            _recipeBook.Recipes.FirstOrDefault(i => i.Id == recipe.Id);

        if (itemToUpdate != null)
        {
            itemToUpdate.Name = recipe.Name;
            itemToUpdate.Servings = recipe.Servings;
            itemToUpdate.TimeToPrepare = recipe.TimeToPrepare;
            itemToUpdate.TimeToCook= recipe.TimeToCook;
            itemToUpdate.Ingredients = recipe.Ingredients;
            itemToUpdate.Directions= recipe.Directions;
            itemToUpdate.Notes = recipe.Notes;
        }
    }

    public void RemoveRecipe(Recipe recipe)
    {
        _recipeBook.Recipes.RemoveAll(i => i.Id == recipe.Id);
        _recipeRepository.RemoveRecipe(recipe);
    }
}