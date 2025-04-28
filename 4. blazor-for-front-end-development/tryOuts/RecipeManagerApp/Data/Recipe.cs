namespace RecipeManagerApp.Data
{
	public class Recipe
	{
		public Guid Id { get; set; }

		public string Name { get; set; } = string.Empty;

		public string Description { get; set; } = string.Empty;

	}
}
