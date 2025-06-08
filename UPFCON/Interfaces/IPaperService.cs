using UPFCON.Models;
using UPFCON.Requests;

namespace UPFCON.Interfaces;


public interface IPaperService
{
    public Task<Paper> CreatePaperAsync(HttpContext httpContext, PaperDto paperDto, Guid eventId);
}