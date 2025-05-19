namespace Shared.Interfaces
{
	public interface IGenericRepository<T>
	{
		Task<T> GetById(int Id);

		Task<T> GetById(string Id);

		Task<IEnumerable<T>> GetAll();
		Task<bool> Add(T entity);
		Task<bool> Update(T entity);
		Task<bool> Delete(T entity);
	}
}
