using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Avalonia.Data;
using Avalonia.Data.Converters;
using HomeChefManager.Models;

namespace HomeChefManager.Converters;

public class RecipeIngredientEnoughQuantity : IMultiValueConverter
{
    public object Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        if (values.Count != 3 || values.Any(v => v == null || v.ToString() == "(unset)"))
            return BindingOperations.DoNothing;

        var result0 = int.TryParse(values[0]!.ToString(), out int recipeIngredientQuantity);
        var result1 = int.TryParse(values[1]!.ToString(), out int ingredientQuantity);
        var result3 = Enum.TryParse(values[2]!.ToString(), out Unit unit);

        if (!result0 || !result1 || !result3)
        {
            return BindingOperations.DoNothing;
        }

        var ingredientQuantityAfterCooking = ingredientQuantity - recipeIngredientQuantity;

        if (ingredientQuantityAfterCooking < 0)
        {
            return $"{ int.Abs(ingredientQuantityAfterCooking) } { unit }s are missing";
        }

        return $"{ int.Abs(ingredientQuantityAfterCooking) } { unit }s left after cooking";
    }
}