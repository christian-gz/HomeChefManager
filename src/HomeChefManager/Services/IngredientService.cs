using System;
using System.Collections.Generic;
using System.Linq;
using HomeChefManager.DataAccess;
using HomeChefManager.Models;

namespace HomeChefManager.Services;

public class IngredientService : IIngredientService
{
    public IngredientService(IIngredientRepository ingredientRepository)
    {
        _ingredientRepository = ingredientRepository;
        _ingredientStorage = new IngredientStorage();

        LoadDb();
    }

    private readonly IngredientStorage _ingredientStorage;
    private readonly IIngredientRepository _ingredientRepository;

    public event EventHandler<IngredientEventArgs>? IngredientsEdited;
    public event EventHandler<IngredientEventArgs>? IngredientQuantityReduced;

    public IEnumerable<Ingredient> GetIngredients()
    {
        return _ingredientStorage.Ingredients;
    }

    private void LoadDb()
    {
        try
        {
            _ingredientStorage.Ingredients = _ingredientRepository.GetIngredients();
        }
        catch (Exception e)
        {
            // Todo: inform user the ingredients could not get loaded
            Console.WriteLine(e.Message);
        }
    }

    public IEnumerable<Ingredient> FilterIngredients(string filterString)
    {
        _ingredientStorage.Ingredients = _ingredientRepository.GetIngredients(filterString);
        return _ingredientStorage.Ingredients;
    }

    public Ingredient GetIngredient(Ingredient ingredient)
    {
        return _ingredientRepository.GetIngredient(ingredient);
    }

    public int AddIngredient(Ingredient ingredient)
    {
        var id = _ingredientRepository.AddIngredient(ingredient);
        _ingredientStorage.Ingredients.Add(ingredient);

        IngredientsEdited?.Invoke(this, new IngredientEventArgs(ingredient));
        return id;
    }

    public void RemoveIngredient(Ingredient ingredient)
    {
        _ingredientRepository.RemoveIngredient(ingredient);
        _ingredientStorage.Ingredients.RemoveAll(i => i.Id == ingredient.Id);

        ingredient.Id = -1;
        IngredientsEdited?.Invoke(this, new IngredientEventArgs(ingredient));
    }

    public void UpdateIngredient(Ingredient ingredient)
    {
        if (ingredient.Quantity < 0)
            return;

        _ingredientRepository.UpdateIngredient(ingredient);

        var itemToUpdate =
            _ingredientStorage.Ingredients.FirstOrDefault(i => i.Id == ingredient.Id);

        if (itemToUpdate != null)
        {
            itemToUpdate.Name = ingredient.Name;
            itemToUpdate.Quantity = ingredient.Quantity;
            itemToUpdate.Unit = ingredient.Unit;
            itemToUpdate.Category = ingredient.Category;
        }

        IngredientsEdited?.Invoke(this, new IngredientEventArgs(ingredient));
    }

    public void ReduceIngredientQuantity(Ingredient ingredient)
    {
        if (ingredient.Quantity < 0)
            return;

        _ingredientRepository.UpdateIngredient(ingredient);

        var itemToUpdate =
            _ingredientStorage.Ingredients.FirstOrDefault(i => i.Id == ingredient.Id);

        if (itemToUpdate != null)
        {
            itemToUpdate.Quantity = ingredient.Quantity;
        }

        IngredientQuantityReduced?.Invoke(this, new IngredientEventArgs(ingredient));
    }
}