using Domain.GameAggregate;

namespace Infrastructure.Repositories;

public class GameRepository : RepositoryBase<Game>
{
    public GameRepository(BaseContext context) : base(context)
    {
    }
}


// пока закомментил из старого репозитория

//using Domain.GameAggregate;
//using Microsoft.EntityFrameworkCore;
//using System.Linq.Expressions;

//namespace Infrastructure;

//public class GameRepository : IBaseRepository<Game>
//{
//    private readonly BaseContext _baseContext;

//    public GameRepository()
//    {
//        _baseContext = new BaseContext();
//    }
//    public IQueryable<Game> FindByCondition(Expression<Func<Game, bool>> expression, bool trackChanges)
//    {
//        return trackChanges ? _baseContext.Games.Where(expression) : _baseContext.Games.Where(expression).AsNoTracking();
//    }

//    public IEnumerable<Game> GetAll()
//    {
//        var games = _baseContext.Games.ToList();
//        return games;
//    }

//    public Game? GetById(int id)
//    {
//        var game = _baseContext.Games.FirstOrDefault(x => x.Id == id);

//        if (game is null)
//            return null;

//        return game;
//    }

//    public Game Add(Game item) => _baseContext.Games.Add(item).Entity;
//    public void AddRange(IEnumerable<Game> items) => _baseContext.Games.AddRange(items);
//    public void Update(Game item) => _baseContext.Games.Update(item);
//    public void Delete(Game item) => _baseContext.Games.Remove(item);
//    public async Task SaveAsync() => await _baseContext.SaveChangesAsync();

//    private bool disposed = false;

//    public virtual void Dispose(bool disposing)
//    {
//        if (!disposed)
//        {
//            if (disposing)
//            {
//                _baseContext.Dispose();
//            }
//        }
//        disposed = true;
//    }

//    public void Dispose()
//    {
//        Dispose(true);
//        GC.SuppressFinalize(this);
//    }
//}