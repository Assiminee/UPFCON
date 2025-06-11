using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UPFCON.Interfaces;
using UPFCON.Models;
using UPFCON.Requests;

namespace UPFCON.Controllers;



[ApiController]
[Authorize]
public class PaperController(IPaperService paperService, 
    IContributorService contributorService) : Controller
{
    private IPaperService PaperService { get; } = paperService;

    private IContributorService ContributorService { get; } = contributorService;

    [HttpPost("/api/v1/events/{eventId:guid}/papers")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> CreatePaper([FromForm] PaperDto paper, [FromRoute] Guid eventId)
    {
        
        var newPaper = await PaperService.CreatePaperAsync(HttpContext, paper, eventId);
        
        await ContributorService.CreateContributorsAsync(HttpContext, newPaper, paper.Contributors);
        
        return Created();
    }
    
    [HttpGet("/api/v1/events/{eventId:guid}/papers/{paperId:guid}")]
    public async Task<IActionResult> GetPaperById( [FromRoute] Guid paperId, [FromRoute] Guid eventId)
    {
        var paper = await PaperService.GetPaperByIdAsync(eventId, paperId);

        return Ok(paper);
    }

    [HttpGet("/api/v1/events/{eventId:guid}/papers")]
    public async Task<IActionResult> GetPapersByEventId( [FromRoute] Guid eventId)
    {
        var papers = await PaperService.GetPapersByEventIdAsync(eventId);
        
        return Ok(papers);
    }

    [HttpDelete("/api/v1/events/{eventId:guid}/papers/{paperId:guid}")]
    public async Task<IActionResult> DeletePaper( [FromRoute] Guid paperId, [FromRoute] Guid eventId)
    {
        Console.WriteLine("1st :"+eventId);
        await PaperService.DeletePaperAsync(HttpContext, eventId, paperId);
        
        return NoContent();
    }
    
    
}