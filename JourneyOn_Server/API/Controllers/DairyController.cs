using Application.Models;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

[ApiController]
[Route("[controller]")]
public class DairyController(
    IdentityApplicationDbContext dbContext,
    UserManager<ApplicationUser> userManager) : ControllerBase
{
    [HttpGet("get-dairy-entries/{userId}")]
    public async Task<ActionResult> GetDairyEntries(int userId)
    {
        var entries = await dbContext.DairyTable.Where(e => e.UserId == userId).ToListAsync();
        return Ok(entries);
    }

    [HttpPost("create-diary-entry")]
    public async Task<ActionResult> CreateDiaryEntry([FromBody] CreateDiaryEntryDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        // Map DTO → Entity
        var entry = DiaryModel.Create(dto.UserId, dto.Title, dto.Body);

        await dbContext.AddAsync(entry);
        await dbContext.SaveChangesAsync();

        // You might want to return the created resource:
        return CreatedAtAction(
            nameof(GetDiaryEntry),
            new { id = entry.Id },
            entry
        );
    }

    [HttpPut("update-dairy-entry/{id}")]
    public async Task<ActionResult> UpdateDiaryEntry(int id, [FromBody] UpdateDiaryEntryDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var entry = await dbContext.DairyTable.FindAsync(id);
        if (entry == null)
            return NotFound(new { Message = "Diary entry not found" });

        // Update mutable fields
        entry.Title     = dto.Title;
        entry.Body      = dto.Body;
        entry.CreatedAt = DateTime.UtcNow;
        entry.UpdatedAt = DateTime.UtcNow;

        await dbContext.SaveChangesAsync();
        return Ok(entry);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<DiaryModel>> GetDiaryEntry(int id)
    {
        var entry = await dbContext.DairyTable.FindAsync(id);
        return entry is null ? NotFound() : Ok(entry);
    }

    [HttpDelete("delete-dairy-entry/{id}")]
    public async Task<ActionResult> DeleteDairyEntry(int id)
    {
        var entry = await dbContext.DairyTable.FindAsync(id);
        if (entry == null)
        {
            return NotFound();
        }

        var result = dbContext.DairyTable.Remove(entry);
        await dbContext.SaveChangesAsync();
        return Ok(result.Entity);
    }


}