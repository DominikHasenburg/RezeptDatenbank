public class Ingredient
{
    public string name {get; set;}
    public string amount{get; set;}

    public Ingredient() { }
    public Ingredient(string newname, string newamount)
    {
        name = newname;
        amount = newamount;
    }
}