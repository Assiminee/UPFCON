using Microsoft.EntityFrameworkCore;

namespace UPFCON.Services;
using UPFCON.Interfaces;
using UPFCON.Models;
using UPFCON.Requests;
using UPFCON.Models.Context;
using System.Security.Claims;   
using System.IdentityModel.Tokens.Jwt;


public class EventService : IEvent
{
    private readonly UpfconContext _context;
    private readonly IHttpContextAccessor _http;


    public EventService(UpfconContext context, IHttpContextAccessor http)
    {
        _context = context;
        _http    = http;

    }

    public async Task<IEnumerable<Event>> GetAllEventsAsync()
    {
        return _context.Events.ToList(); 
    }

    public async Task<Event?> GetEventByIdAsync(Guid id)
    {
        return await _context.Events.FindAsync(id);
    }
    
    private Guid? GetCurrentChairmanId()
    {
        var ctx = _http.HttpContext;
        if (ctx is null || !ctx.User.Identity?.IsAuthenticated == true)
            return null;

        string? idStr = ctx.User.FindFirstValue(ClaimTypes.NameIdentifier) // "nameidentifier"
                        ?? ctx.User.FindFirstValue(JwtRegisteredClaimNames.Sub) // "sub"
                        ?? ctx.User.FindFirstValue("sub")                      // brut
                        ?? ctx.User.FindFirstValue("uid");                     // au cas où

        return Guid.TryParse(idStr, out var id) ? id : null;
    }
    private async Task<Guid?> EnsureCurrentChairmanAsync()
    {
        var userId = GetCurrentChairmanId();
        if (userId is null) return null;

        var chairman = await _context.Chairmans
            .FirstOrDefaultAsync(c => c.UserId == userId);
        if (chairman == null)
        {
            chairman = new Chairman { UserId = userId.Value };
            _context.Chairmans.Add(chairman);
          
        }
        foreach (var cl in _http.HttpContext!.User.Claims)
        {
            Console.WriteLine($"CLAIM: {cl.Type} = {cl.Value}");
        }


        return chairman.UserId;
    }


    public async Task<Event> CreateEventAsync(EventDto dto,Guid creatorId)
    {
        var newEvent = new Event
        {
            Id        = Guid.NewGuid(),
            Title     = dto.Title,
            Acronym   = dto.Acronym,
            EventType = dto.EventType,
            StartsAt  = dto.StartsAt,
            EndsAt    = dto.EndsAt,
            Theme     = dto.Theme,
            Location  = dto.Location,
            Topics    = dto.Topics,
            SubTopics = dto.SubTopics,
            Logo      = dto.Logo,
            Description = dto.Description,
            OrganizingCommittee = new List<CommitteeMember>(),
            SubmissionRules = new SubmissionRules
            {
                EventId           = Guid.Empty,
                Font              = dto.SubmissionRules.Font,
                MinPages          = dto.SubmissionRules.MinPages,
                MaxPages          = dto.SubmissionRules.MaxPages,
                Formats           = dto.SubmissionRules.Formats,
                Margins           = dto.SubmissionRules.Margins,
                LineSpacing       = dto.SubmissionRules.LineSpacing,
                AdditionalRules   = dto.SubmissionRules.AdditionalRules,
                FileNameFormat    = dto.SubmissionRules.FileNameFormat,
                SubmissionDeadline= dto.SubmissionRules.SubmissionDeadline
                
            }
        };
        newEvent.OrganizingCommittee.Add(new CommitteeMember
        {
            ChairmanId       = creatorId,
            Role             = CommitteeMemberRole.HeadChairman,
            InvitedAt        = DateTime.UtcNow,
            InvitationStatus = InvitationStatusEnum.Accepted.ToString()
        });

        foreach (var cm in dto.CommitteeMembers
                     .Where(cm => creatorId == null || cm.ChairmanId != creatorId))
        {
            newEvent.OrganizingCommittee.Add(new CommitteeMember
            {
                ChairmanId       = cm.ChairmanId,
                Role             = cm.Role,
                InvitedAt        = DateTime.UtcNow,
                InvitationStatus = Enum.GetName(InvitationStatusEnum.PendingResponse)!
            });
        }

        _context.Events.Add(newEvent);
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            Console.WriteLine(ex.InnerException?.Message);
            throw;
        }
        return newEvent;
    }


    public async Task<Event?> UpdateEventAsync(Guid id, EventDto dto)
    {
        var existing = await _context.Events
            .Include(e => e.SubmissionRules)
            .FirstOrDefaultAsync(e => e.Id == id);
        if (existing == null) return null;

        /* champs simples */
        existing.Title       = dto.Title;
        existing.Acronym     = dto.Acronym;
        existing.EventType   = dto.EventType;
        existing.StartsAt    = dto.StartsAt;
        existing.EndsAt      = dto.EndsAt;
        existing.Theme       = dto.Theme;
        existing.Location    = dto.Location;
        existing.Topics      = dto.Topics;
        existing.SubTopics   = dto.SubTopics;
        existing.Logo        = dto.Logo;
        existing.Description = dto.Description;

        if (existing.SubmissionRules is null)
        {
            existing.SubmissionRules = new SubmissionRules
            {
                EventId           = existing.Id,                       // FK
                Font              = dto.SubmissionRules.Font,
                MinPages          = dto.SubmissionRules.MinPages,
                MaxPages          = dto.SubmissionRules.MaxPages,
                Formats           = dto.SubmissionRules.Formats,
                Margins           = dto.SubmissionRules.Margins,
                LineSpacing       = dto.SubmissionRules.LineSpacing,
                AdditionalRules   = dto.SubmissionRules.AdditionalRules,
                FileNameFormat    = dto.SubmissionRules.FileNameFormat,
                SubmissionDeadline= dto.SubmissionRules.SubmissionDeadline
            };
        }

        var sr = existing.SubmissionRules;
        sr.Font              = dto.SubmissionRules.Font;
        sr.MinPages          = dto.SubmissionRules.MinPages;
        sr.MaxPages          = dto.SubmissionRules.MaxPages;
        sr.Formats           = dto.SubmissionRules.Formats;
        sr.Margins           = dto.SubmissionRules.Margins;
        sr.LineSpacing       = dto.SubmissionRules.LineSpacing;
        sr.AdditionalRules   = dto.SubmissionRules.AdditionalRules;
        sr.FileNameFormat    = dto.SubmissionRules.FileNameFormat;
        sr.SubmissionDeadline= dto.SubmissionRules.SubmissionDeadline;

        await _context.SaveChangesAsync();
        return existing;
    }


    public async Task<bool> DeleteEventAsync(Guid id)
    {
        var ev = await _context.Events.FindAsync(id);
        if (ev == null) return false;

        _context.Events.Remove(ev);
        await _context.SaveChangesAsync();
        return true;
    }
}