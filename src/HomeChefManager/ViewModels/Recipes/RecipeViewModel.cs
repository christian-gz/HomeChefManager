using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using HomeChefManager.Models;
using HomeChefManager.ViewModels.Ingredients;
using ReactiveUI;

namespace HomeChefManager.ViewModels.Recipes;

public class RecipeViewModel : ViewModelBase
{
    private int? _id;
    private string _name;
    private int? _servings;
    private int? _timeToPrepare;
    private int? _timeToCook;
    private ObservableCollection<IngredientQuantity> _ingredients;
    private string? _directions;
    private string? _notes;

    public int? Id
    {
        get => _id;
        set => this.RaiseAndSetIfChanged(ref _id, value);
    }

    public string Name
    {
        get => _name;
        set => this.RaiseAndSetIfChanged(ref _name, value);
    }

    public int? Servings
    {
        get => _servings;
        set => this.RaiseAndSetIfChanged(ref _servings, value);
    }

    public int? TimeToPrepare
    {
        get => _timeToPrepare;
        set => this.RaiseAndSetIfChanged(ref _timeToPrepare, value);
    }

    public int? TimeToCook
    {
        get => _timeToCook;
        set => this.RaiseAndSetIfChanged(ref _timeToCook, value);
    }

    public ObservableCollection<IngredientQuantity> Ingredients
    {
        get => _ingredients;
        set => this.RaiseAndSetIfChanged(ref _ingredients, value);
    }

    public string? Directions
    {
        get => _directions;
        set => this.RaiseAndSetIfChanged(ref _directions, value);
    }

    public string? Notes
    {
        get => _notes;
        set => this.RaiseAndSetIfChanged(ref _notes, value);
    }

    public RecipeViewModel(string name, ObservableCollection<IngredientQuantity>? ingredients = null, int? id = null)
    {
        _id = id;
        _name = name;
        _ingredients = ingredients ?? new ObservableCollection<IngredientQuantity>();
    }

    public RecipeViewModel(Recipe recipe)
    {
        _id = recipe.Id;
        _name = recipe.Name;
        _servings = recipe.Servings;
        _timeToPrepare = recipe.TimeToPrepare;
        _timeToCook = recipe.TimeToCook;
        _ingredients = new ObservableCollection<IngredientQuantity>(
            recipe.Ingredients.Select(i => new IngredientQuantity(new IngredientViewModel(i.Key), i.Value))
        );
        _directions = recipe.Directions;
        _notes = recipe.Notes;
    }

    public Recipe ToRecipe()
    {
        var ingredients = Ingredients.ToDictionary(iq => iq.IngredientViewModel.ToIngredient(), iq => iq.Quantity);
        return new Recipe(Name, ingredients, Id)
        {
            Servings = Servings,
            TimeToPrepare = TimeToPrepare,
            TimeToCook = TimeToCook,
            Directions = Directions,
            Notes = Notes,
        };
    }

    public override string ToString()
    {
        var recipe = $"ID: {Id}, Name: {Name}, Servings: {Servings}, Ingredients: \n";
        foreach (var ingredient in Ingredients)
        {
            recipe += $"{ingredient.Quantity} {ingredient.IngredientViewModel.Unit} of {ingredient.IngredientViewModel.Name}\n";
        }
        return recipe;
    }
}