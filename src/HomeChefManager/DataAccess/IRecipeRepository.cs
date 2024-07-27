using System.Collections.Generic;
using HomeChefManager.Models;

namespace HomeChefManager.DataAccess;

public interface IRecipeRepository
{
    List<Recipe> LoadRecipes();
    Recipe GetRecipeDetails(Recipe recipe);
    int AddRecipe(Recipe recipe);
    void UpdateRecipe(Recipe recipe);
    void RemoveRecipe(Recipe recipe);
}