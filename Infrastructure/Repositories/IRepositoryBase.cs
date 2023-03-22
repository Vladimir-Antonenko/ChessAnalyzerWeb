using System.Linq.Expressions;

namespace Infrastructure.Repositories;

public interface IRepositoryBase<T> where T : class
{
    IQueryable<T> FindAll(bool trackChanges);
    IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression, bool trackChanges);
    T Add(T item);
    void Update(T item);
    void Delete(T item);
    Task Save();
}

// старый репозиторий
//using System.Linq.Expressions;

//namespace Infrastructure;

//internal interface IBaseRepository<T> : IDisposable where T : class
//{
//    public IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression, bool trackChanges);
//    public IEnumerable<T> GetAll();
//    public T? GetById(int id);
//    public T Add(T item);
//    public void AddRange(IEnumerable<T> items);
//    public void Update(T item);
//    public void Delete(T item);
//    public Task SaveAsync();
//}