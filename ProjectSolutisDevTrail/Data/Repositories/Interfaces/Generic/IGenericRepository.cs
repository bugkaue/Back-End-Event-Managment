namespace ProjectSolutisDevTrail.Data.Repositories.Interfaces.generic;

public interface IGenericRepository<T> where T : class
{
    Task<IEnumerable<T>> GetAllAsync();
    Task<T> GetByIdAsync(int id);
    Task<T> AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(int id);
    Task<IEnumerable<T>> GetByIdsAsync(IEnumerable<int> ids);
}
