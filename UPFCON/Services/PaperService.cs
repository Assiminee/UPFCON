using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UPFCON.Exceptions;
using UPFCON.Interfaces;
using UPFCON.Models;
using UPFCON.Models.Context;
using UPFCON.Requests;
using UPFCON.Responses;

namespace UPFCON.Services;



public class PaperService(UserManager<User> userManager, UpfconContext upfconContext,
    IUserService userService, IWebHostEnvironment env)  : IPaperService
{
    private IWebHostEnvironment Env { get; } = env;
    private const long MaxFileSize  = 5 * 1024 * 1024;
    
    private UserManager<User> UserManager { get; } = userManager;
    
    private UpfconContext Context { get; } = upfconContext;
    
    private IUserService UserService { get; } = userService;


    public async Task<Paper> CreatePaperAsync(HttpContext httpContext, PaperDto paperDto, Guid eventId)
    {
        var email = httpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

        if (email == null)
            throw new NotFoundException("Email not found");
        
        var loggedUser = await UserService.FindUserByEmail(email);
        
        var isAuthor = await UserManager.IsInRoleAsync(loggedUser,"Author");
        
        if(!isAuthor)
            throw new ForbiddenException("Forbidden : only an Author can create a Paper ");
        
        
        if (string.IsNullOrWhiteSpace(paperDto.Title))
            throw new InvalidFileException($"Incorrect paper title: {paperDto.Title}");
        
        if (paperDto.PaperFile.Length == 0)
            throw new InvalidFileException($"Invalid file size: 0Mb");
        
        var filePath = await SavePaperFileAsync(paperDto.PaperFile);
        
        var @event = await Context.Events.FirstOrDefaultAsync(e => e.Id == eventId);
        if (@event == null)
            throw new NotFoundException("Event not found");

        var paper = new Paper()
        {
            Title = paperDto.Title,
            Abstract = paperDto.Abstract,
            Keywords = paperDto.Keywords,
            PublicationDate = paperDto.PublicationDate,
            SubmittedAt = DateTime.Today,
            Path = filePath,
            EventId = eventId, 
            Event = @event,
            Status =  Enum.GetName(PaperStatus.PendingEvaluation)!
        };
        var s =  await Context.Papers.AddAsync(paper);
        await Context.SaveChangesAsync();
        
        @event.SubmittedPapers.Add(paper);
        await Context.SaveChangesAsync();

        return s.Entity;
    }

    public async Task<PaperResponseDto> GetPaperByIdAsync(Guid eventId, Guid paperId)
    {
        var e = await Context.Events.FirstOrDefaultAsync(e => e.Id == eventId) ??
            throw new NotFoundException("Event not found");

        
        var paper = await Context.Papers.Where(p => p.EventId == eventId && p.Id == paperId)
            .Include(paper => paper.Event)
            .Include(p => p.Contributors).ThenInclude(contribution => contribution.Author)
            .ThenInclude(author => author.User)
            .Include(p => p.Evaluations)
            .Include(p => p.TimeSlot)
            .FirstOrDefaultAsync() ?? throw new NotFoundException("Paper not found");

        var names = new List<string>();
        foreach (var contributor in paper.Contributors)
        {
            names.Add(contributor.Author.User.FullName + " : " + contributor.Role);
        }
        return new PaperResponseDto()
        {
            Id = paperId,
            Title = paper.Title,
            Abstract = paper.Abstract,
            Path = paper.Path,
            Keywords = paper.Keywords,
            PublicationDate = paper.PublicationDate,
            SubmittedAt = paper.SubmittedAt,
            Status = paper.Status,
            EventId = eventId,
            EventName = paper.Event.Title,
            ContributorsNames = names,
            Evaluations = paper.Evaluations,
        };
    }

    public async Task<List<PaperResponseDto>> GetPapersByEventIdAsync(Guid eventId)
    {

        var e = await Context.Events.FirstOrDefaultAsync(e => e.Id == eventId) ??
                throw new NotFoundException("Event not found");
        
        var papers = await Context.Papers.Where(p => p.EventId == eventId)
            .Include(paper => paper.Event)
            .Include(p => p.Contributors).ThenInclude(contribution => contribution.Author)
            .ThenInclude(author => author.User)
            .Include(p => p.Evaluations)
            .Include(p => p.TimeSlot)
            .ToListAsync() ;
        
        var papersDto = new List<PaperResponseDto>();
        foreach (var paper in papers)
        {
            var names = new List<string>();
            foreach (var contributor in paper.Contributors)
            {
                names.Add(contributor.Author.User.FullName + " : " + contributor.Role);
            }
            
            var paperDto = new PaperResponseDto()
            {
                Id=paper.Id,
                Title = paper.Title,
                Abstract = paper.Abstract,
                Path = paper.Path,
                Keywords = paper.Keywords,
                PublicationDate = paper.PublicationDate,
                SubmittedAt = paper.SubmittedAt,
                Status = paper.Status,
                EventId = eventId,
                EventName = paper.Event.Title,
                ContributorsNames = names,
                Evaluations = paper.Evaluations,
            };
            papersDto.Add(paperDto);
        }
        return papersDto;
    }

    public async Task DeletePaperAsync(HttpContext httpContext, Guid eventId, Guid paperId)
    {
        var email = httpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

        if (email == null)
            throw new NotFoundException("Email not found");
        
        var loggedUser = await UserService.FindUserByEmail(email);
      //  await Context.Entry(loggedUser).Reference(u => u.Author).LoadAsync();
        
      
        Console.WriteLine("2nd: "+eventId);
        var e = await Context.Events.FirstOrDefaultAsync(e => e.Id == eventId) ??
                throw new NotFoundException("Event not found");
        Console.WriteLine("event name : " +e.Title);
        
        var paper = await Context.Papers.Where(p => p.EventId == eventId && p.Id == paperId)
            .Include(p => p.Contributors).ThenInclude(contribution => contribution.Author)
            .FirstOrDefaultAsync() ?? throw new NotFoundException("Paper not found");

        var headAuthor = paper.Contributors.FirstOrDefault(c =>
            c.Role == nameof(ContributorRole.HeadAuthor)) ?? throw new NotFoundException("no HeadAuthor found");
        
        if(headAuthor.AuthorId != loggedUser.Id)
            throw new ForbiddenException("You are not authorized to delete this paper, Only the headAuthor can Delete it");
        
        Context.Papers.Remove(paper);
        await Context.SaveChangesAsync();
    }


    private async Task<string> SavePaperFileAsync(IFormFile file)
    {
        var allowedExtensions = new [] {".jpg", ".jpeg", ".png", ".pdf"};
        
        var fileExtension = Path.GetExtension(file.FileName).ToLower();

        if (!allowedExtensions.Contains(fileExtension))
            throw new InvalidFileException($"Invalid file extension: '{fileExtension}'");

        if (file.Length > MaxFileSize)
            throw new InvalidFileException($"Invalid file size: {file.Length}");
        
        var fileName = Guid.NewGuid() + fileExtension;
        
        var uploadsFolder = CreateUploadsDirectoryIfNotExist();
        
        var filePath = Path.Combine(uploadsFolder, fileName);
        using (var fileStream = new FileStream(filePath, FileMode.Create))
            await file.CopyToAsync(fileStream);
        
        return filePath;
    }

    private string CreateUploadsDirectoryIfNotExist()
    {
        var uploadsFolder = Path.Combine(Env.WebRootPath, "uploads", "papers");
        
        if (!Directory.Exists(uploadsFolder))
            Directory.CreateDirectory(uploadsFolder);
        
        return uploadsFolder;
    }
}