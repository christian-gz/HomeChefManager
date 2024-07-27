using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Input;
using DynamicData.Kernel;
using HomeChefManager.Models;
using HomeChefManager.ViewModels.Ingredients;
using ReactiveUI;

namespace HomeChefManager.ViewModels.Recipes;

public class RecipeDialogBaseViewModel : ViewModelBase, INotifyDataErrorInfo
{
    public RecipeDialogBaseViewModel()
    {
        NoErrors = this.WhenAnyValue(vm => vm.HasErrors, hasErrors => !hasErrors);
        CancelCommand = ReactiveCommand.Create(Cancel);
        AddIngredientCommand = ReactiveCommand.Create(AddIngredient);
    }

    private string _name;
    private string? _servings;
    private string? _timeToPrepare;
    private string? _timeToCook;
    private string? _directions;
    private string? _notes;
    private string? _ingredientsError;
    
    private static readonly string UnitsPattern = string.Join("|", Enum.GetNames(typeof(Unit)));
    protected static readonly string Pattern = $@"\s\(({UnitsPattern})\)$";

    private readonly Dictionary<string, List<ValidationResult>> _errors = new();

    public event EventHandler<RecipeEventArgs>? RecipeCanceled;
    public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;
    public bool HasErrors => _errors.Count > 0;
    protected readonly IObservable<bool> NoErrors;
    public ICommand CancelCommand { get; }
    public ICommand AddIngredientCommand { get; }

    public ObservableCollection<IngredientViewModel> Ingredients { get; set; }

    public ObservableCollection<IngredientStringQuantity> IngredientStringQuantities { get; set; } = new();

    public string Name
    {
        get => _name;
        set
        {
            this.RaiseAndSetIfChanged(ref _name, value);
            Validate_Name();
        }
    }

    public string? Servings
    {
        get => _servings;
        set
        {
            this.RaiseAndSetIfChanged(ref _servings, value);
            Validate_Servings();
        }
    }

    public string? TimeToPrepare
    {
        get => _timeToPrepare;
        set
        {
            this.RaiseAndSetIfChanged(ref _timeToPrepare, value);
            Validate_TimeToPrepare();
        }
    }

    public string? TimeToCook
    {
        get => _timeToCook;
        set
        {
            this.RaiseAndSetIfChanged(ref _timeToCook, value);
            Validate_TimeToCook();
        }
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

    public string? IngredientsError
    {
        get => _ingredientsError;
        set => this.RaiseAndSetIfChanged(ref _ingredientsError, value);
    }

    private void Validate_Name()
    {
        ClearErrors(nameof(Name));

        if (string.IsNullOrEmpty(Name))
        {
            AddError(nameof(Name), "Please enter a name for the recipe.");
        }
    }
    private void Validate_Servings()
    {
        ClearErrors(nameof(Servings));

        if (string.IsNullOrEmpty(Servings))
        {
            return;
        }

        bool result = int.TryParse(Servings, out int number);

        if (!result)
        {
            AddError(nameof(Servings), "Please enter a valid number.");
            return;
        }

        if (int.Parse(Servings) < 0)
        {
            AddError(nameof(Servings), "Please enter a positive number.");
        }

        if (int.Parse(Servings) > 100)
        {
            AddError(nameof(Servings), "Please enter a reasonable number of servings.");
        }
    }
    private void Validate_TimeToPrepare()
    {
        ClearErrors(nameof(TimeToPrepare));

        if (string.IsNullOrEmpty(TimeToPrepare))
        {
            return;
        }

        bool result = int.TryParse(TimeToPrepare, out int number);

        if (!result)
        {
            AddError(nameof(TimeToPrepare), "Please enter a valid number.");
            return;
        }

        if (int.Parse(TimeToPrepare) < 0)
        {
            AddError(nameof(TimeToPrepare), "Please enter a positive number.");
        }

        if (int.Parse(TimeToPrepare) > 1000)
        {
            AddError(nameof(TimeToPrepare), "Please enter a realistic cooking time");
        }
    }
    private void Validate_TimeToCook()
    {
        ClearErrors(nameof(TimeToCook));

        if (string.IsNullOrEmpty(TimeToCook))
        {
            return;
        }

        bool result = int.TryParse(TimeToCook, out int number);

        if (!result)
        {
            AddError(nameof(TimeToCook), "Please enter a valid number.");
            return;
        }

        if (int.Parse(TimeToCook) < 0)
        {
            AddError(nameof(TimeToCook), "Please enter a positive number.");
        }

        if (int.Parse(TimeToCook) > 1000)
        {
            AddError(nameof(TimeToCook), "Please enter a realistic preparation time.");
        }
    }
    private void Validate_Ingredients()
    {
        ClearErrors(nameof(IngredientsError));
        IngredientsError = "";

        var ingredientStringList = new List<string>();

        foreach (var item in IngredientStringQuantities)
        {
            if (string.IsNullOrEmpty(item.SelectedIngredient))
            {
                continue;
            }

            ingredientStringList.Add(item.SelectedIngredient);
        }

        ingredientStringList.Sort();
        var duplicates = ingredientStringList.Duplicates(s => s);

        if (duplicates.Any())
        {
            var errorMessage = "Please remove the duplicate ingredients";
            AddError(nameof(IngredientsError), errorMessage);
            IngredientsError = errorMessage;
        }
    }

    protected void ValidateAll()
    {
        Validate_Name();
        Validate_Ingredients();
    }

    private void Cancel()
    {
        Name = "";
        Servings = null;
        TimeToPrepare = null;
        TimeToCook = null;
        Directions = null;
        Notes = null;
        IngredientStringQuantities.Clear();

        ClearErrors();

        RecipeCanceled?.Invoke(this, new RecipeEventArgs(null));
    }

    private void AddIngredient()
    {
        var ingredientSelection = new IngredientStringQuantity();
        ingredientSelection.SelectedIngredientChanged += Validate_Ingredients;
        IngredientStringQuantities.Add(ingredientSelection);
    }

    protected IEnumerable<IngredientQuantity> IngredientSelectionToIngredientQuantity(IEnumerable<IngredientStringQuantity> ingredientStringQuantities)
    {
        List<IngredientQuantity> ingredientQuantities = new List<IngredientQuantity>();

        foreach (var item in ingredientStringQuantities)
        {
            if (string.IsNullOrWhiteSpace(item.SelectedIngredient) || item.Quantity <= 0 || item.Quantity == null)
            {
                continue;
            }

            var nameCleared = Regex.Replace(item.SelectedIngredient, Pattern, "");
            var ingredientViewModel = Ingredients.FirstOrDefault(i => string.Equals(i.Name, nameCleared));

            if (ingredientViewModel == null)
            {
                continue;
            }

            var ingredientQuantity = new IngredientQuantity(ingredientViewModel, (int)item.Quantity);
            ingredientQuantities.Add(ingredientQuantity);
        }

        return ingredientQuantities;
    }

    protected void ClearDialog()
    {
        Name = "";
        Servings = null;
        TimeToPrepare = null;
        TimeToCook = null;
        Directions = null;
        Notes = null;
        IngredientStringQuantities.Clear();

        ClearErrors();
    }

    public IEnumerable GetErrors(string? propertyName)
    {
        if (string.IsNullOrEmpty(propertyName))
        {
            return _errors.Values.SelectMany(static errors => errors);
        }

        if (_errors.TryGetValue(propertyName!, out List<ValidationResult>? result))
        {
            return result;
        }

        return Array.Empty<ValidationResult>();
    }

    private void ClearErrors(string? propertyName = null)
    {
        if (string.IsNullOrEmpty(propertyName))
        {
            _errors.Clear();
        }
        else
        {
            _errors.Remove(propertyName);
        }

        ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        this.RaisePropertyChanged(nameof(HasErrors));

        IngredientsError = "";
    }

    private void AddError(string propertyName, string errorMessage)
    {
        if (!_errors.TryGetValue(propertyName, out List<ValidationResult>? propertyErrors))
        {
            propertyErrors = new List<ValidationResult>();
            _errors.Add(propertyName, propertyErrors);
        }

        propertyErrors.Add(new ValidationResult(errorMessage));

        ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        this.RaisePropertyChanged(nameof(HasErrors));
    }
}