using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Windows.Input;
using HomeChefManager.Models;
using ReactiveUI;

namespace HomeChefManager.ViewModels.Ingredients;

public class IngredientDialogBaseViewModel : ViewModelBase, INotifyDataErrorInfo
{
    protected IngredientDialogBaseViewModel()
    {
        NoErrors = this.WhenAnyValue(vm => vm.HasErrors, hasErrors => !hasErrors);
        CancelCommand = ReactiveCommand.Create(Cancel);
    }
    
    private string _name;
    private string _quantity;
    private Unit? _unit;
    private string? _category;
    private readonly Dictionary<string, List<ValidationResult>> _errors = new();

    public event EventHandler<IngredientViewModelEventArgs>? IngredientCanceled;
    public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

    public bool HasErrors => _errors.Count > 0;
    protected readonly IObservable<bool> NoErrors;
    public Unit[] Units { get; } = Enum.GetValues(typeof(Unit)).Cast<Unit>().ToArray();
    public ICommand CancelCommand { get; }

    public string Name
    {
        get => _name;
        set
        {
            this.RaiseAndSetIfChanged(ref _name, value);
            Validate_Name();
        }
    }

    public string Quantity
    {
        get => _quantity;
        set
        {
            this.RaiseAndSetIfChanged(ref _quantity, value);
            Validate_Quantity();
        }
    }

    public Unit? Unit
    {
        get => _unit;
        set
        {
            this.RaiseAndSetIfChanged(ref _unit, value);
            Validate_Unit();
        }
    }

    public string? Category
    {
        get => _category;
        set => this.RaiseAndSetIfChanged(ref _category, value);
    }

    private void Validate_Name()
    {
        ClearErrors(nameof(Name));

        if (string.IsNullOrEmpty(Name))
        {
            AddError(nameof(Name), "Please enter a name for the ingredient.");
        }
    }

    private void Validate_Quantity()
    {
        ClearErrors(nameof(Quantity));

        bool result = int.TryParse(Quantity, out int number);

        if (!result)
        {
            AddError(nameof(Quantity), "Please enter a valid number.");
            return;
        }

        if (int.Parse(Quantity) < 0)
        {
            AddError(nameof(Quantity), "Please enter a positive number.");
        }

        if (int.Parse(Quantity) > 1000000)
        {
            AddError(nameof(Quantity), "Please enter a reasonable quantity.");
        }
    }

    private void Validate_Unit()
    {
        ClearErrors(nameof(Unit));

        if (Unit == null)
        {
            AddError(nameof(Unit), "Please enter a unit for the ingredient.");
        }
    }

    protected void ValidateAll()
    {
        Validate_Unit();
        Validate_Quantity();
        Validate_Name();
    }

    private void Cancel()
    {
        _name = "";
        _quantity = "";
        _unit = null;
        _category = null;
        ClearErrors();

        IngredientCanceled?.Invoke(this, new IngredientViewModelEventArgs(null));
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

    protected void ClearErrors(string? propertyName = null)
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