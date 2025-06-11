namespace UPFCON.Interfaces;

using UPFCON.Models;
using UPFCON.Requests;
public interface IEvent
{
    Task<IEnumerable<Event>> GetAllEventsAsync();
    Task<Event?> GetEventByIdAsync(Guid id);
    Task<Event> CreateEventAsync(EventDto dto,Guid creatorId);
    Task<Event?> UpdateEventAsync(Guid id, EventDto dto);
    Task<bool> DeleteEventAsync(Guid id);
}