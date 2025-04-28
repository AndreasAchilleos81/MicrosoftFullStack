
namespace RecipeManagerApp.Data
{
	public interface IRecipeRepo
	{
		Task<List<Recipe>> GetAllRecipesAsync();

		Task AddRecipe(string name, string description);

	    Task AddRecipe(Recipe recipe);
	}
}