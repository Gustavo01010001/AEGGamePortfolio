using backend.Data;
using backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GamesController : ControllerBase
{
    private readonly AppDbContext _db;
    public GamesController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Game>>> GetAll() =>
        await _db.Games.AsNoTracking().ToListAsync();

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Game>> GetById(int id)
    {
        var game = await _db.Games.FindAsync(id);
        return game is null ? NotFound() : Ok(game);
    }

    [HttpPost]
    public async Task<ActionResult<Game>> Create([FromBody] Game game)
    {
        _db.Games.Add(game);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = game.Id }, game);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] Game game)
    {
        if (id != game.Id) return BadRequest();
        _db.Entry(game).State = EntityState.Modified;
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var game = await _db.Games.FindAsync(id);
        if (game is null) return NotFound();
        _db.Games.Remove(game);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
