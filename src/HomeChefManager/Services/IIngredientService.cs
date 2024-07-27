using System;
using System.Collections.Generic;
using HomeChefManager.Models;

namespace HomeChefManager.Services;

public interface IIngredientService
{
    IEnumerable<Ingredient> GetIngredients();
    IEnumerable<Ingredient> FilterIngredients(string filterString);
    Ingredient GetIngredient(Ingredient ingredient);
    int AddIngredient(Ingredient ingredient);
    void RemoveIngredient(Ingredient ingredient);
    void UpdateIngredient(Ingredient ingredient);
    void ReduceIngredientQuantity(Ingredient ingredient);

    event EventHandler<IngredientEventArgs>? IngredientsEdited;
    event EventHandler<IngredientEventArgs>? IngredientQuantityReduced;
}