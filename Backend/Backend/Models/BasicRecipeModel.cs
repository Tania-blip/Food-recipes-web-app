namespace Backend.Models;

public class BasicRecipeModel
{
    public string? Id { get; set; }
    public string? Name { get; set; }
    public string? Author { get; set; }
    public int? NoIngredients { get; set; }
    public string? SkillLevel { get; set; }
}