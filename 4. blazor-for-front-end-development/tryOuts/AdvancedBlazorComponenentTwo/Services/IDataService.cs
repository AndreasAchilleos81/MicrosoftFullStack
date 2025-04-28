using AdvancedBlazorComponenentTwo.DataTypes;

namespace AdvancedBlazorComponenentTwo.Services
{
	public interface IDataService
	{
		Task<List<Person>> GetData();
	}
}