using System.Collections;
using UPFCON.Models;

namespace UPFCON.Interfaces;

public interface IAuthorService
{
    public  Task<IEnumerable> GetAuthors(HttpContext httpContext);
}