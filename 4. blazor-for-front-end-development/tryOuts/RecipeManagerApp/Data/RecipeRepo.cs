using Blazored.LocalStorage;
using RecipeManagerApp.Data;
using System.Globalization;
using System.Runtime.InteropServices;

namespace RecipeManagerApp.Data
{
	public class RecipeRepo : IRecipeRepo
	{
		private readonly ILocalStorageService _localStorageService;
		private const string StorageKey = "recipes";

		public RecipeRepo(ILocalStorageService localStorageService)
		{
			_localStorageService = localStorageService;
		}

		public async Task PopulateRecipesStorage()
		{
			List<Recipe> _recipes = new List<Recipe>
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
			// check if there are already any recipes
			var currentStorage = await GetAllRecipesAsync();
			if (currentStorage == null || currentStorage.Count == 0)
			{
				await _localStorageService.SetItemAsync(StorageKey, _recipes);
			}
		}

		public async Task<List<Recipe>> GetAllRecipesAsync()
		{
			var recipes = await _localStorageService.GetItemAsync<List<Recipe>>(StorageKey);
			return recipes ?? new List<Recipe>();
		}

		public async Task AddRecipe(string name, string description)
		{
			var newRecipe = new Recipe
			{
				Id = Guid.NewGuid(),
				Name = name,
				Description = description
			};

			var recipes = await GetAllRecipesAsync();
			recipes.Add(newRecipe);
			await _localStorageService.SetItemAsync(StorageKey, recipes);

		}

		public async Task AddRecipe(Recipe recipe)
		{
			recipe.Id = Guid.NewGuid();
			var recipes = await GetAllRecipesAsync();
			recipes.Add(recipe);
			await _localStorageService.SetItemAsync(StorageKey, recipes);
		}

		public async Task<Recipe> GetRecipeById(Guid id)
		{
			var recipies = await GetAllRecipesAsync();
			var recipe = recipies.FirstOrDefault(r => r.Id == id);
			return recipe;
		}

		public async Task DeleteRecipe(Guid id)
		{
			var recipes = await GetAllRecipesAsync();
			recipes.RemoveAll(r => r.Id == id);
			await _localStorageService.SetItemAsync(StorageKey, recipes);
		}
	}
}