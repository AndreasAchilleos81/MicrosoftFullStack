using RecipeManagerApp.Data;
using System.Globalization;

namespace RecipeManagerApp.Data
{
	public class RecipeRepo : IRecipeRepo
	{
		private List<Recipe> _recipes = new List<Recipe>
			{
				new Recipe
				{
					Id = Guid.NewGuid(),
					Name = "Spaghetti Bolognese",
					Description = "A classic Italian pasta dish with a rich meat sauce."
				},
				new Recipe
				{
					Id = Guid.NewGuid(),
					Name = "Chicken Curry",
					Description = "A spicy and flavorful chicken curry with aromatic spices."
				},
				new Recipe
				{
					Id = Guid.NewGuid(),
					Name = "Beef Stroganoff",
					Description = "A creamy Russian dish with sautéed beef and mushrooms."
				},
				new Recipe
				{
					Id = Guid.NewGuid(),
					Name = "Vegetable Stir Fry",
					Description = "A quick and healthy stir fry with fresh vegetables."
				},
				new Recipe
				{
					Id = Guid.NewGuid(),
					Name = "Margherita Pizza",
					Description = "A simple Italian pizza with fresh tomatoes, mozzarella, and basil."
				},
				new Recipe
				{
					Id = Guid.NewGuid(),
					Name = "Caesar Salad",
					Description = "A classic salad with romaine lettuce, croutons, and Caesar dressing."
				}
			};
		public RecipeRepo()
		{

		}
		public async Task<List<Recipe>> GetAllRecipesAsync()
		{
			await Task.Delay(500); // Simulate a delay for async operation
			return _recipes;
		}

		public async Task AddRecipe(string name, string description)
		{
			await Task.Delay(500); // Simulate a delay for async operation
			_recipes.Add(new Recipe
			{
				Id = Guid.NewGuid(),
				Name = name,
				Description = description
			});
		}

		public async Task AddRecipe(Recipe recipe)
		{
			await Task.Delay(500); // Simulate a delay for async operation
			_recipes.Add(recipe);
		}
	}
}



/*
    public class RecipeRepo
   {
       private readonly ILocalStorageService _localStorage;
       private const string StorageKey = "recipes";

       public RecipeRepo(ILocalStorageService localStorage)
       {
           _localStorage = localStorage;
       }

       public async Task<List<Recipe>> GetAllRecipesAsync()
       {
           var recipes = await _localStorage.GetItemAsync<List<Recipe>>(StorageKey);
           return recipes ?? new List<Recipe>();
       }

       public async Task AddRecipe(Recipe recipe)
       {
           var recipes = await GetAllRecipesAsync();
           recipes.Add(recipe);
           await _localStorage.SetItemAsync(StorageKey, recipes);
       }
   }
 
 */