using HomeChefManager.Models;
using ReactiveUI;

namespace HomeChefManager.ViewModels.Ingredients;

public class IngredientViewModel : ReactiveObject
{
    private int? _id;
    private string _name;
    private int _quantity;
    private Unit _unit;
    private string? _category;

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

    public int Quantity
    {
        get => _quantity;
        set => this.RaiseAndSetIfChanged(ref _quantity, value);
    }

    public Unit Unit
    {
        get => _unit;
        set => this.RaiseAndSetIfChanged(ref _unit, value);
    }

    public string? Category
    {
        get => _category;
        set => this.RaiseAndSetIfChanged(ref _category, value);
    }

    public IngredientViewModel(string name, int quantity, Unit? unit = null, string? category = null, int? id = null)
    {
        _id = id;
        _name = name;
        _quantity = quantity;
        _unit = unit ?? Unit.Gram;
        _category = category;
    }

    public IngredientViewModel(Ingredient ingredient)
    {
        _id = ingredient.Id;
        _name = ingredient.Name;
        _quantity = ingredient.Quantity;
        _unit = ingredient.Unit;
        _category = ingredient.Category;
    }

    public Ingredient ToIngredient()
    {
        return new Ingredient(Id, Name, Quantity, Unit) { Category = Category };
    }

    public override string ToString()
    {
        return $"{ Name } ({ Unit })";
    }
}