//namespace WebApplication1.Controllers;

using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Neo4j.Driver;
using WebApplication1.Properties;
using Backend.Models;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RecipesController : ControllerBase
    {

        [HttpGet("getAll")]
        public async Task<IActionResult> GetRecipes([FromQuery]int page, [FromQuery]int limit)
        {
            var session = MyNeo4j.Driver.AsyncSession();
            try
            {
                var skip = limit * page + 1;
                var query = @"
                MATCH (a:Author)-[:WROTE]->(r:Recipe)-[:CONTAINS_INGREDIENT]->(i:Ingredient)
                RETURN r.id AS RecipeId, r.name AS RecipeName, r.skillLevel AS skillLevel, a.name AS AuthorName, count(i) AS Ingredients
                ORDER BY r.name ASC
                SKIP $skip
                LIMIT $limit";
                var result2 = await session.RunAsync(query, new{skip = skip, limit = limit});
                var recipes = await result2.ToListAsync(record =>
                {
                    var node = record;

                    return new BasicRecipeModel
                    {
                        Id = node.Get<string>("RecipeId"),
                        Name = node.Get<string>("RecipeName"),
                        Author = node.Get<string>("AuthorName"),
                        NoIngredients = node.Get<int>("Ingredients"),
                        SkillLevel = node.Get<string>("skillLevel"),

                    };
                });

                return Ok(recipes);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return NotFound(e.Message);
            }
            finally
            {
                await session.CloseAsync();
            }
        }

        [HttpGet("get/{id}")]
        public async Task<IActionResult> GetRecipe(string id)
        {
            var session = MyNeo4j.Driver.AsyncSession();
            try
            {
                string query = @"
                MATCH (r:Recipe {id: $recipeId})-[:CONTAINS_INGREDIENT]->(i:Ingredient)
                RETURN r.name AS Name, r.description AS Description, r.cookingTime AS CookingTime, r.preparationTime AS PreparationTime, collect(i.name) AS Ingredients";

                var result = await session.RunAsync(query, new { recipeId = id });
                var record = await result.SingleAsync();
                if (record != null)
                {
                    Console.WriteLine(id);
                    var recipe = new ExtendedRecipeModel()
                    {
                        Name = record.Get<string>("Name"),
                        Description = record.Get<string>("Description"),
                        CookingTime = record.Get<int>("CookingTime"),
                        PreparationTime = record.Get<int>("PreparationTime"),
                        Ingredients = record.Get<List<string>>("Ingredients"),
                    };
                    return Ok(recipe);
                }

                return NotFound($"Recipe with ID {id} not found.");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return NotFound(e.Message);
            }
            finally
            {
                await session.CloseAsync();
            }
        }


        [HttpGet("search/{name}")]
        public async Task<IActionResult> GetRecipesByName(string name)
        {
            var session = MyNeo4j.Driver.AsyncSession();
            try
            {
                string query = @"
                    MATCH (r:Recipe)-[:CONTAINS_INGREDIENT]->(i:Ingredient)
                    WHERE r.name CONTAINS $RecipeName
                    WITH r, count(i) as IngredientsCount
                    MATCH (a:Author)-[:WROTE]->(r)
                    RETURN r.id AS RecipeId, r.name AS RecipeName, a.name AS AuthorName, 
                           IngredientsCount AS Ingredients, r.skillLevel AS SkillLevel";

                var result = await session.RunAsync(query, new { RecipeName = name });

                var recipes = await result.ToListAsync(record =>
                {
                    return new BasicRecipeModel
                    {
                        Id = record.Get<string>("RecipeId"),
                        Name = record.Get<string>("RecipeName"),
                        Author = record.Get<string>("AuthorName"),
                        NoIngredients = record.Get<int>("Ingredients"),
                        SkillLevel = record.Get<string>("SkillLevel")
                    };
                });

                if (recipes.Count == 0)
                {
                    return NotFound("No recipes found matching the specified name.");
                }

                return Ok(recipes);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return StatusCode(500, "An error occurred while processing your request.");
            }
            finally
            {
                await session.CloseAsync();
            }
        }

        [HttpGet("filterByIngredients")]
        public async Task<IActionResult> GetRecipesByIngredients([FromQuery] List<string> ingredients)
        {
            if (ingredients == null || !ingredients.Any())
            {
                return BadRequest("At least one ingredient must be specified.");
            }

            var session = MyNeo4j.Driver.AsyncSession();
            try
            {
                var query = @"
                MATCH (r:Recipe)-[:CONTAINS_INGREDIENT]->(i:Ingredient)
                WHERE i.name IN $IngredientList
                WITH r, COLLECT(i.name) AS IngredientsIncluded
                WHERE ALL(ing IN $IngredientList WHERE ing IN IngredientsIncluded)
                RETURN r.id AS RecipeId, r.name AS RecipeName, IngredientsIncluded
                ORDER BY SIZE(IngredientsIncluded) DESC";

                var result = await session.RunAsync(query, new { IngredientList = ingredients });

                var recipes = await result.ToListAsync(record =>
                {
                    return new
                    {
                        Id = record.Get<string>("RecipeId"),
                        Name = record.Get<string>("RecipeName"),
                        Ingredients = record.Get<List<string>>("IngredientsIncluded")
                    };
                });

                if (recipes.Count == 0)
                {
                    return NotFound("No recipes found containing all specified ingredients.");
                }

                return Ok(recipes);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return StatusCode(500, "An error occurred while processing your request.");
            }
            finally
            {
                await session.CloseAsync();
            }
        }
        
        [HttpGet("authorRecipes/{authorName}")]
        public async Task<IActionResult> GetRecipesByAuthor(string authorName)
        {
            var session = MyNeo4j.Driver.AsyncSession();
            try
            {
                var query = @"
            MATCH (a:Author {name: $AuthorName})-[:WROTE]->(r:Recipe)
            RETURN r.name AS RecipeName, r.description AS Description, 
                   COLLECT(r) AS Recipes";

                var result = await session.RunAsync(query, new { AuthorName = authorName });

                var recipes = await result.ToListAsync(record =>
                {
                    return new
                    {
                        Name = record.Get<string>("RecipeName"),
                    };
                });

                if (recipes.Count == 0)
                {
                    return NotFound($"No recipes found for author {authorName}.");
                }

                return Ok(recipes);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return StatusCode(500, "An error occurred while processing your request.");
            }
            finally
            {
                await session.CloseAsync();
            }
        }

    }
}


    

