namespace Backend.Models;

public class ExtendedRecipeModel : BasicRecipeModel
{
    public string? Description { get; set; }
    public int? CookingTime { get; set; }
    public int? PreparationTime { get; set; }
    
    public List<string>? Ingredients { get; set; }
}