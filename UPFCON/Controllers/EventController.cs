using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UPFCON.Interfaces;
using UPFCON.Models;
using UPFCON.Requests;

namespace UPFCON.Controllers;



[ApiController]
[Route("/api/v1/events")]
[Authorize]
public class EventController(IPaperService paperService, IContributorService contributorService) : Controller
{
    private IPaperService PaperService { get; } = paperService;

    private IContributorService ContributorService { get; } = contributorService;

    [HttpPost("{eventId}/papers")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> CreatePaper([FromForm] PaperDto paper, [FromRoute] Guid eventId)
    {
        
        var newPaper = await PaperService.CreatePaperAsync(HttpContext, paper, eventId);
        Console.WriteLine("paper created");
        var contributions = await ContributorService.CreateContributorsAsync(HttpContext,
                 newPaper, paper.Contributors );
        Console.WriteLine("contributions created");
        
        return Created();
    }
    
    
}