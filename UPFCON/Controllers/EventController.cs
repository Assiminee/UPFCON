using Microsoft.AspNetCore.Authorization;

namespace UPFCON.Controllers;
using Microsoft.AspNetCore.Mvc;
using UPFCON.Interfaces;
using UPFCON.Requests;

[ApiController]
[Route("api/v1/auth/events")]
[Authorize]
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

    [HttpPost("upload-logo")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UploadLogo([FromForm] IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("File is empty");

        var folderPath = Path.Combine("wwwroot", "uploads", "logos");
        Directory.CreateDirectory(folderPath);

        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
        var filePath = Path.Combine(folderPath, fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        var publicPath = $"/uploads/logos/{fileName}";
        return Ok(new { path = publicPath });
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