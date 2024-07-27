using System.Collections.Generic;
using HomeChefManager.Models;

namespace HomeChefManager.DataAccess;

public interface IIngredientRepository
{
    List<Ingredient> GetIngredients(string filterString = "");
    Ingredient GetIngredient(Ingredient ingredient);
    int AddIngredient(Ingredient ingredient);
    void RemoveIngredient(Ingredient ingredient);
    void UpdateIngredient(Ingredient ingredient);
}