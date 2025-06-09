namespace UPFCON.Services;
using UPFCON.Interfaces;
using UPFCON.Models;
using UPFCON.Requests;
using UPFCON.Models.Context;


public class EventService : IEvent
{
    private readonly UpfconContext _context;

    public EventService(UpfconContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Event>> GetAllEventsAsync()
    {
        return _context.Events.ToList(); 
    }

    public async Task<Event?> GetEventByIdAsync(Guid id)
    {
        return await _context.Events.FindAsync(id);
    }

    public async Task<Event> CreateEventAsync(EventDto dto)
    {
        var newEvent = new Event
        {
            Id = Guid.NewGuid(),
            Title = dto.Title,
            Acronym = dto.Acronym,
            EventType = dto.EventType,
            StartsAt = dto.StartsAt,
            EndsAt = dto.EndsAt,
            Theme = dto.Theme,
            Location = dto.Location,
            Topics = dto.Topics,
            SubTopics = dto.SubTopics,
            Logo = dto.Logo,
            Description = dto.Description
        };

        _context.Events.Add(newEvent);
        await _context.SaveChangesAsync();
        return newEvent;
    }

    public async Task<Event?> UpdateEventAsync(Guid id, EventDto dto)
    {
        var existing = await _context.Events.FindAsync(id);
        if (existing == null) return null;

        existing.Title = dto.Title;
        existing.Acronym = dto.Acronym;
        existing.EventType = dto.EventType;
        existing.StartsAt = dto.StartsAt;
        existing.EndsAt = dto.EndsAt;
        existing.Theme = dto.Theme;
        existing.Location = dto.Location;
        existing.Topics = dto.Topics;
        existing.SubTopics = dto.SubTopics;
        existing.Logo = dto.Logo;
        existing.Description = dto.Description;

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