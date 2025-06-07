namespace UPFCON.Controllers;
using Microsoft.AspNetCore.Mvc;
using UPFCON.Interfaces;
using UPFCON.Requests;

[ApiController]
[Route("api/v1/events")]
public class EventController : ControllerBase
{
    private readonly IEvent _service;

    public EventController(IEvent service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _service.GetAllEventsAsync());

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var ev = await _service.GetEventByIdAsync(id);
        return ev == null ? NotFound() : Ok(ev);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] EventDto dto)
    {
        Console.WriteLine("im here");
        var created = await _service.CreateEventAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] EventDto dto)
    {
        var updated = await _service.UpdateEventAsync(id, dto);
        return updated == null ? NotFound() : Ok(updated);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        return await _service.DeleteEventAsync(id) ? NoContent() : NotFound();
    }
}