using Domain;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Infrastructure.Repositories;

public abstract class RepositoryBase<T> : IRepositoryBase<T> where T : class
{
    protected BaseContext Context { get; set; }

    protected RepositoryBase(BaseContext context) => Context = context;

    public IQueryable<T> FindAll(bool trackChanges) => trackChanges ? Context.Set<T>() : Context.Set<T>().AsNoTracking();

    public IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression, bool trackChanges) =>
        trackChanges ? Context.Set<T>().Where(expression) : Context.Set<T>().Where(expression).AsNoTracking();

    public T Add(T item) => Context.Set<T>().Add(item).Entity;

   // public void Update(T item) => Context.Set<T>().Update(item);

    public void Delete(T item) => Context.Set<T>().Remove(item);

    public async Task Save() => await Context.SaveChangesAsync();
}