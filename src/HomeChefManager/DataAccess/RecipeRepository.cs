using System;
using System.Collections.Generic;
using System.Linq;
using Dapper;
using HomeChefManager.Configuration;
using HomeChefManager.Models;

namespace HomeChefManager.DataAccess;

public class RecipeRepository : GenericRepository<RecipeDto>, IRecipeRepository
{
    public RecipeRepository(IConfig config) : base(config) { }

    public List<Recipe> LoadRecipes()
    {
        var sql = "SELECT Recipes.Id, Recipes.Name, Recipes.Servings, Recipes.TimeToPrepare, Recipes.TimeToCook FROM Recipes";
        var recipes = Query(sql).Select(dto => new Recipe (dto.Name, new Dictionary<Ingredient, int>(), dto.Id)
        {
            Servings = dto.Servings,
            TimeToPrepare = dto.TimeToPrepare,
            TimeToCook = dto.TimeToCook,
            Directions = "",
            Notes = ""
        });

        return recipes.ToList();
    }

    public Recipe GetRecipeDetails(Recipe recipe)
    {
        if (recipe.Id == null)
        {
            throw new Exception("Recipe ID can't be null");
        }

        recipe.Ingredients.Clear();

        using (var connection = GetConnection())
        {
            connection.Query<RecipeDto, RecipeIngredientDto, IngredientDto, Recipe>(
                @"SELECT r.*, ri.Quantity AS IngredientQuantity, i.*
                     FROM Recipes r
                     LEFT JOIN RecipeIngredients ri ON r.Id = ri.RecipeId
                     LEFT JOIN Ingredients i ON ri.IngredientId = i.Id
                     WHERE r.Id = @Id",
                (recipeDto, recipeIngredientDto, ingredientDto) =>
                {
                    if (ingredientDto != null)
                    {
                        var ingredient = new Ingredient (ingredientDto.Id, ingredientDto.Name, ingredientDto.Quantity, Enum.Parse<Unit>(ingredientDto.Unit))
                        {
                            Category = ingredientDto.Category
                        };

                        recipe.Ingredients.Add(ingredient, recipeIngredientDto.IngredientQuantity);
                    }

                    if (recipeDto != null)
                    {
                        recipe.Directions = recipeDto.Directions;
                        recipe.Notes = recipeDto.Notes;
                    }

                    return recipe;
                },
                new { Id = recipe.Id},
                splitOn: "IngredientQuantity,Id");

            return recipe;
        }
    }

    public int AddRecipe(Recipe recipe)
    {
        using (var connection = GetConnection())
        {
            connection.Open();
            using (var transaction = connection.BeginTransaction())
            {
                try
                {
                    var sql = "INSERT INTO Recipes (Name, Servings, TimeToPrepare, TimeToCook, Directions, Notes)" +
                              "VALUES (@Name, @Servings, @TimeToPrepare, @TimeToCook, @Directions, @Notes);" +
                              "SELECT last_insert_rowid();";

                    var recipeDto = new RecipeDto
                    {
                        Name = recipe.Name,
                        Servings = recipe.Servings,
                        TimeToPrepare = recipe.TimeToPrepare,
                        TimeToCook = recipe.TimeToCook,
                        Directions = recipe.Directions,
                        Notes = recipe.Notes
                    };

                    var id = connection.QuerySingle<int>(sql, recipeDto, transaction);
                    recipe.Id = id;

                    foreach (var item in recipe.Ingredients)
                    {
                        if (item.Key.Id == null)
                        {
                            continue;
                        }

                        sql = "INSERT INTO RecipeIngredients (RecipeId, IngredientId, Quantity)" +
                              "VALUES (@RecipeId, @IngredientId, @IngredientQuantity);";

                        var recipeIngredientDto = new RecipeIngredientDto
                        {
                            RecipeId = (int)recipe.Id,
                            IngredientId = (int)item.Key.Id,
                            IngredientQuantity = item.Value
                        };

                        connection.Execute(sql, recipeIngredientDto, transaction);
                    }
                    transaction.Commit();
                    return id;
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }
    }

    public void UpdateRecipe(Recipe recipe)
    {
        if (recipe.Id == null)
        {
            throw new Exception("Recipe ID can't be null");
        }

        using (var connection = GetConnection())
        {
            connection.Open();
            using (var transaction = connection.BeginTransaction())
            {
                try
                {
                    var recipeDto = new RecipeDto
                    {
                        Id = (int)recipe.Id,
                        Name = recipe.Name,
                        Servings = recipe.Servings,
                        TimeToPrepare = recipe.TimeToPrepare,
                        TimeToCook = recipe.TimeToCook,
                        Directions = recipe.Directions,
                        Notes = recipe.Notes
                    };

                    var sql = "UPDATE Recipes SET Name = @Name," +
                                    "Servings = @Servings, TimeToPrepare = @TimeToPrepare," +
                                    "TimeToCook = @TimeToCook, Directions = @Directions," +
                                    "Notes = @Notes WHERE Id = @Id;";

                    connection.Execute(sql, recipeDto, transaction);

                    connection.Execute("DELETE FROM RecipeIngredients WHERE RecipeId = @Id;",
                        new { Id = recipe.Id }, transaction);

                    foreach (var item in recipe.Ingredients)
                    {
                        if (item.Key.Id == null)
                        {
                            continue;
                        }

                        sql = "INSERT INTO RecipeIngredients (RecipeId, IngredientId, Quantity)" +
                              "VALUES (@RecipeId, @IngredientId, @IngredientQuantity);";

                        var recipeIngredientDto = new RecipeIngredientDto
                        {
                            RecipeId = (int)recipe.Id,
                            IngredientId = (int)item.Key.Id,
                            IngredientQuantity = item.Value
                        };

                        connection.Execute(sql, recipeIngredientDto, transaction);
                    }

                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }
    }

    public void RemoveRecipe(Recipe recipe)
    {
        if (recipe.Id == null)
        {
            throw new Exception("Recipe ID can't be null");
        }

        var sql = "DELETE FROM Recipes WHERE Id = @Id;";
        Remove(sql, new { Id = recipe.Id });
    }
}