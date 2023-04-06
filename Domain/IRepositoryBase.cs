using System.Linq.Expressions;

namespace Domain;

/// <summary>
/// Интерфейс базового репозитория для работы с данными
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IRepositoryBase<T> where T : class
{
    IQueryable<T> FindAll(bool trackChanges);
    IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression, bool trackChanges);
    T Add(T item);
    void Update(T item);
    void Delete(T item);
    Task Save();
}