using System.Collections;
using UPFCON.Models;
using UPFCON.Requests;

namespace UPFCON.Interfaces;

public interface IAuthorService
{
    
    public Task<(IList<User> users, IEnumerable<string> roles)> CreateAuthors(HttpContext httpContext, List<AuthorRegisterDto> authorRegisterDtos);
    public  Task<IEnumerable> GetAuthors(HttpContext httpContext);
}