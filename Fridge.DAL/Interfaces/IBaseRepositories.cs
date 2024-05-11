namespace Fridge.DAL.Interfaces
{
    public interface IBaseRepositories<T>
    {
        Task Create(T entity);
        IQueryable<T> Get();
        Task Delete(T entity);
        Task<T> Update(T entity);
    }
}
