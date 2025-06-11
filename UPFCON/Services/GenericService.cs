using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using UPFCON.Interfaces;

namespace UPFCON.Services;

public class GenericService : IGenericService
{
    public async Task<PageResult<T>> GetPagedResultAsync<T>(
        IQueryable<T> query, int page, int pageSize,
        Expression<Func<T, object>>? orderBy = null,
        Expression<Func<T, bool>>? where = null
    ) where T : class
    {
        if (where != null)
            query = query.Where(where);
        
        var total = await query.CountAsync();

        if (orderBy != null)
            query = query.OrderBy(orderBy);
        
        var items = await query.Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PageResult<T> { Items = items, Count = total };
    }
}