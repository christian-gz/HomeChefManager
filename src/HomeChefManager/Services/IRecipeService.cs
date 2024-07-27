using System.Collections.Generic;
using HomeChefManager.Models;

namespace HomeChefManager.Services;

public interface IRecipeService
{
   IEnumerable<Recipe> GetRecipes();
   Recipe GetRecipeDetails(Recipe recipe);
   int AddRecipe(Recipe recipe);
   void UpdateRecipe(Recipe recipe);
   void RemoveRecipe(Recipe recipe);
}