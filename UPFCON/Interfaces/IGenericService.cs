using System.Linq.Expressions;

namespace UPFCON.Interfaces;

public class PageResult<T>
{
    public IList<T> Items { get; set; } = new List<T>();
    public int Count { get; set; }
}

public interface IGenericService
{
    Task<PageResult<T>> GetPagedResultAsync<T>(
        IQueryable<T> query, int page, int pageSize,
        Expression<Func<T, object>> orderBy,
        Expression<Func<T, bool>>? where = null
        ) where T : class;
}