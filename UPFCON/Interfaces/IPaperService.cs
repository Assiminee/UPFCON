using UPFCON.Models;
using UPFCON.Requests;
using UPFCON.Responses;

namespace UPFCON.Interfaces;


public interface IPaperService
{
    public Task<Paper> CreatePaperAsync(HttpContext httpContext, PaperDto paperDto, Guid eventId);
    
    public Task<PaperResponseDto> GetPaperByIdAsync(Guid eventId, Guid paperId);
    
    public Task<List<PaperResponseDto>> GetPapersByEventIdAsync(Guid eventId);
    
    public Task DeletePaperAsync(HttpContext httpContext,Guid eventId, Guid paperId);
}