using System.Collections.Generic;
public class Recipe
{
    public string id { get; set; }
    public string name { get; set; }
    public List<Ingredient> ingredients { get; set; }

    public Recipe() { }
    public Recipe(string id, string newname, List<Ingredient> initialingredients)
    {
        name = newname;
        ingredients = initialingredients;
    }

    public void addIngredient(string i, string a)
    {
        ingredients.Add(new Ingredient(i, a));
    }
    public void removeIngredient(int index)
    {
        ingredients.RemoveAt(index);
    }
}