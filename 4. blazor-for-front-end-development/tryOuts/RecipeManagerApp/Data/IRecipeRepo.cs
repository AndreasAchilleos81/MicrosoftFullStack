
namespace RecipeManagerApp.Data
{
	public interface IRecipeRepo
	{
		Task PopulateRecipesStorage();

		Task<List<Recipe>> GetAllRecipesAsync();

		Task AddRecipe(string name, string description);

	    Task AddRecipe(Recipe recipe);

		Task<Recipe> GetRecipeById(Guid id);

		Task DeleteRecipe(Guid id);
	}
}