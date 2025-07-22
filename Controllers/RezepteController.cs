using System.Data.SQLite;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System;
namespace RezeptDatenbank.Controllers
{
    [ApiController]
    [Route("api/recipes")]
    public class RecipesController : ControllerBase
    {
        const string connectionString = "Data Source=RecipesLocal.db;";
        SQLiteConnection connection = new SQLiteConnection(connectionString);

        public RecipesController()
        {
            connection.Open();
            var command = new SQLiteCommand(connection);

            command.CommandText = """
            CREATE TABLE IF NOT EXISTS "Rezepte" (
                "Id" TEXT NOT NULL,
                "Name" TEXT NOT NULL,
                PRIMARY KEY("Name","Id")
            );
            CREATE TABLE IF NOT EXISTS "Zutaten" (
                "Name"	TEXT NOT NULL,
                "Rezept"	TEXT NOT NULL,
                "Menge"	TEXT NOT NULL,
                PRIMARY KEY("Name","Rezept")
            );
            """;

            command.ExecuteNonQuery();
        }

        [HttpGet]
        public IActionResult ViewRecipes()
        {
            List<Recipe> recipes = loadRecipesList();
            return Ok(recipes);
        }
        public List<Recipe> loadRecipesList()
        {
            var command = new SQLiteCommand(connection);
            command.CommandText = "SELECT * FROM Rezepte";
            var reader = command.ExecuteReader();
            List<string> recipeNames = [];
            while (reader.Read())
            {
                recipeNames.Add(reader.GetString(1));
            }
            reader.Close();
            List<Recipe> recipes = [];
            foreach (string name in recipeNames)
            {
                recipes.Add(loadRecipe(name));
            }
            return recipes;
        }

        private Recipe loadRecipe(string name)
        {
            List<Ingredient> ingredients = [];
            string id;
            using (var command = new SQLiteCommand(connection))
            {
                command.CommandText = "SELECT * FROM Zutaten WHERE Rezept=@name";
                command.Parameters.AddWithValue("@name", name);
                using (var reader = command.ExecuteReader())
                {

                    while (reader.Read())
                    {
                        ingredients.Add(new Ingredient(reader.GetString(0), reader.GetString(2)));
                    }
                }
            }
            using (var command = new SQLiteCommand(connection))
            {
                command.CommandText = "SELECT * FROM Rezepte WHERE Name=@name";
                command.Parameters.Add(new SQLiteParameter("@name", name));
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read()) { id = reader.GetString(0); }
                    else { id = ""; }
                }
            }
            return new Recipe(id, name, ingredients);
        }
        [HttpPost]
        public IActionResult Add([FromBody] Recipe rezept)
        {
            try
            {
                var recipes = loadRecipesList();
                int id;
                if (recipes.Count > 0)
                {
                    id = recipes.Count+1;
                }
                else
                {
                    id = 1;
                    Console.WriteLine("id set as one because List was empty");
                }
                AddRecipe(id.ToString(), rezept.name, rezept.ingredients);
                return CreatedAtAction("ViewRecipes", rezept);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Fehler im POST Handler: " + ex.Message);
                return BadRequest(new { error = ex.Message });
            }
        }
        private void AddRecipe(string id, string name, List<Ingredient> ingredients)
        {
            using var insertRezept = new SQLiteCommand(connection);
            insertRezept.CommandText = "INSERT INTO Rezepte (Id, Name) VALUES (@id, @name)";
            insertRezept.Parameters.AddWithValue("@id", id);
            insertRezept.Parameters.AddWithValue("@name", name);
            insertRezept.ExecuteNonQuery();

            foreach (var ingredient in ingredients)
            {
                using var insertZutat = new SQLiteCommand(connection);
                insertZutat.CommandText = "INSERT INTO Zutaten (Name, Rezept, Menge) VALUES (@name, @recipe, @amount)";
                insertZutat.Parameters.AddWithValue("@name", ingredient.name);
                insertZutat.Parameters.AddWithValue("@recipe", name);
                insertZutat.Parameters.AddWithValue("@amount", ingredient.amount);
                insertZutat.ExecuteNonQuery();
            }
        }
    }
}