using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Windows.Input;
using HomeChefManager.Models;
using HomeChefManager.Services;
using ReactiveUI;

namespace HomeChefManager.ViewModels.Ingredients;

public class IngredientListViewModel : ViewModelBase
{
    public IngredientListViewModel()
    {
        IsAddIngredientDialogOpen = false;

        IngredientList = new ObservableCollection<IngredientViewModel>
        {
            new IngredientViewModel(new Ingredient(100, "Banana", 200, Unit.Gram)),
            new IngredientViewModel(new Ingredient(101, "Milk", 2000, Unit.Milliliter) { Category = "Organic" })
        };
    }
    public IngredientListViewModel(IIngredientService ingredientService)
    {
        _ingredientService = ingredientService;
        _ingredientService.IngredientQuantityReduced += HandleIngredientQuantityReduced;
        IngredientList = new ObservableCollection<IngredientViewModel>();

        foreach (var ingredient in _ingredientService.GetIngredients())
        {
            IngredientList.Add(new IngredientViewModel(ingredient));
        }

        AddIngredientViewModel = new AddIngredientViewModel();
        AddIngredientViewModel.IngredientAdded += HandleIngredientAdded;
        AddIngredientViewModel.IngredientCanceled += HandleIngredientCanceled;

        DetailIngredientViewModel = new DetailIngredientViewModel();
        DetailIngredientViewModel.IngredientEdited += HandleIngredientEdited;
        DetailIngredientViewModel.IngredientCanceled += HandleIngredientCanceled;
        DetailIngredientViewModel.IngredientRemoved += HandleIngredientRemoved;

        OpenAddIngredientCommand = ReactiveCommand.Create(() => IsAddIngredientDialogOpen = true);
        OpenDetailIngredientCommand = ReactiveCommand.Create<IngredientViewModel>(entry => OpenDetailIngredient(entry));

        this.WhenAnyValue(x => x.FilterString)
            .Throttle(TimeSpan.FromSeconds(0.3))
            .Select(filterString => filterString.Trim())
            .DistinctUntilChanged()
            .Select(filterString => _ingredientService.FilterIngredients(filterString))
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(ingredients =>
            {
                IngredientList.Clear();
                foreach (var ingredient in ingredients)
                {
                    IngredientList.Add(new IngredientViewModel(ingredient));
                }
            });
    }

    private readonly IIngredientService _ingredientService;
    private bool _isAddIngredientDialogOpen;
    private bool _isDetailIngredientDialogOpen;
    private string _filterString = "";

    public ObservableCollection<IngredientViewModel> IngredientList { get; }
    public AddIngredientViewModel AddIngredientViewModel { get; }
    public DetailIngredientViewModel DetailIngredientViewModel { get; }
    public ICommand OpenAddIngredientCommand { get; }
    public ICommand OpenDetailIngredientCommand { get; }

    public bool IsAddIngredientDialogOpen
    {
        get => _isAddIngredientDialogOpen;
        set => this.RaiseAndSetIfChanged(ref _isAddIngredientDialogOpen, value);
    }
    public bool IsDetailIngredientDialogOpen
    {
        get => _isDetailIngredientDialogOpen;
        set => this.RaiseAndSetIfChanged(ref _isDetailIngredientDialogOpen, value);
    }
    public string FilterString
    {
        get => _filterString;
        set => this.RaiseAndSetIfChanged(ref _filterString, value);
    }

    private void HandleIngredientAdded(object? o, IngredientViewModelEventArgs e)
    {
        if (e.IngredientViewModel != null)
        {
            AddIngredient(e.IngredientViewModel);
        }

        IsAddIngredientDialogOpen = false;
    }
    private void HandleIngredientEdited(object? o, IngredientViewModelEventArgs e)
    {
        if (e.IngredientViewModel != null)
        {
            UpdateIngredient(e.IngredientViewModel);
        }

        IsDetailIngredientDialogOpen = false;
    }
    private void HandleIngredientRemoved(object? o, IngredientViewModelEventArgs e)
    {
        if (e.IngredientViewModel != null)
        {
            RemoveIngredient(e.IngredientViewModel);
        }

        IsDetailIngredientDialogOpen = false;
    }

    private void HandleIngredientCanceled(object? o, IngredientViewModelEventArgs e)
    {
        if (e.IngredientViewModel == null)
        {
            IsAddIngredientDialogOpen = false;
            IsDetailIngredientDialogOpen = false;
        }
    }

    private void HandleIngredientQuantityReduced(object? o, IngredientEventArgs e)
    {
        if (e.Ingredient != null)
        {
            var ingredient = e.Ingredient;
            var ingredientToUpdate = IngredientList.FirstOrDefault(i => i.Id == ingredient.Id);

            if (ingredientToUpdate != null)
            {
                ingredientToUpdate.Quantity = ingredient.Quantity;
            }
        }
    }

    public void AddIngredient(IngredientViewModel ingredientViewModel)
    {
        try
        {
            int id = _ingredientService.AddIngredient(ingredientViewModel.ToIngredient());
            ingredientViewModel.Id = id;
            IngredientList.Add(ingredientViewModel);
        }
        catch (Exception e)
        {
            // Todo: inform user
            Console.WriteLine(e.Message);
        }
    }

    public void RemoveIngredient(IngredientViewModel ingredientViewModel)
    {
        try
        {
            _ingredientService.RemoveIngredient(ingredientViewModel.ToIngredient());
            IngredientList.Remove(ingredientViewModel);
        }
        catch (Exception e)
        {
            // Todo: inform user
            Console.WriteLine(e.Message);
        }
    }

    private void UpdateIngredient(IngredientViewModel ingredientViewModel)
    {
        try
        {
            _ingredientService.UpdateIngredient(ingredientViewModel.ToIngredient());
        }
        catch (Exception e)
        {
            // Todo: inform user
            Console.WriteLine(e);
        }
    }

    private void OpenDetailIngredient(IngredientViewModel ingredientViewModel)
    {
        IsDetailIngredientDialogOpen = true;
        DetailIngredientViewModel.CurrentIngredientViewModel = ingredientViewModel;
    }
}