using System;
using System.Collections.Generic;
using System.Linq;
using HomeChefManager.Configuration;
using HomeChefManager.Models;
using HomeChefManager.Utils;

namespace HomeChefManager.DataAccess;

public class IngredientRepository : GenericRepository<IngredientDto>, IIngredientRepository
{
    public IngredientRepository(IConfig config) : base(config) { }

    public List<Ingredient> GetIngredients(string filterString = "")
    {
        var sql = "SELECT * FROM Ingredients";

        if (!string.IsNullOrEmpty(filterString))
        {
            sql += " WHERE Name LIKE @SearchPattern" +
                   " OR Category LIKE @SearchPattern";
        }

        var ingredients = Query(sql, new { SearchPattern = $"%{filterString}%" })
            .Select(dto => new Ingredient(dto.Id, dto.Name, dto.Quantity, Enum.Parse<Unit>(dto.Unit))
            {
                Category = dto.Category
            });

        return ingredients.ToList();
    }

    public Ingredient GetIngredient(Ingredient ingredient)
    {
        if (ingredient.Id == null)
        {
            throw new Exception("Ingredient ID can't be null");
        }

        var sql = "SELECT * FROM Ingredients WHERE Id = @Id";
        var ingredientDto = Query(sql, new { Id = ingredient.Id }).FirstOrDefault();

        if (ingredientDto == null)
        {
            throw new RecordNotFoundException(
                $"Ingredient { ingredient.Name } with the ID { ingredient.Id } not found."
            );
        }

        ingredient.Name = ingredientDto.Name;
        ingredient.Quantity = ingredientDto.Quantity;
        ingredient.Unit = Enum.Parse<Unit>(ingredientDto.Unit);
        ingredient.Category = ingredientDto.Category;

        return ingredient;
    }

    public int AddIngredient(Ingredient ingredient)
    {
        var sql = "INSERT INTO Ingredients (Name, Quantity, Unit, Category) VALUES (@Name, @Quantity, @Unit, @Category);" +
                       "SELECT last_insert_rowid();";
        var ingredientDto = new IngredientDto {
            Name = ingredient.Name,
            Quantity = ingredient.Quantity,
            Unit = ingredient.Unit.ToString(),
            Category = ingredient.Category
        };
        var id = Add(sql, ingredientDto);
        ingredient.Id = id;

        return id;
    }

    public void RemoveIngredient(Ingredient ingredient)
    {
        if (ingredient.Id == null)
        {
            throw new Exception("Ingredient ID can't be null");
        }

        var sql = "DELETE FROM Ingredients WHERE Id = @Id;";
        Remove(sql, new { Id = ingredient.Id });
    }

    public void UpdateIngredient(Ingredient ingredient)
    {
        if (ingredient.Id == null)
        {
            throw new Exception("Ingredient ID can't be null");
        }

        var ingredientDto = new IngredientDto
        {
            Id = (int)ingredient.Id,
            Name = ingredient.Name,
            Quantity = ingredient.Quantity,
            Unit = ingredient.Unit.ToString(),
            Category = ingredient.Category
        };

        var sql = "UPDATE Ingredients SET Name = @Name," +
                       "Quantity = @Quantity, Unit = @Unit," +
                       "Category = @Category Where Id = @Id";

        Update(sql, ingredientDto);
    }
}