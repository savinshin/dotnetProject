using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TodosController : ControllerBase
{
    private readonly AppDbContext _db;

    public TodosController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Todo>>> GetAll()
        => Ok(await _db.Todos.AsNoTracking().ToListAsync());

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Todo>> GetOne(int id)
    {
        var todo = await _db.Todos.AsNoTracking().FirstOrDefaultAsync(t => t.Id == id);
        return todo is null ? NotFound() : Ok(todo);
    }

    [HttpPost]
    public async Task<ActionResult<Todo>> Create([FromBody] Todo input)
    {
        _db.Todos.Add(input);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetOne), new { id = input.Id }, input);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] Todo input)
    {
        var todo = await _db.Todos.FindAsync(id);
        if (todo is null) return NotFound();

        todo.Title = input.Title;
        todo.IsDone = input.IsDone;
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var todo = await _db.Todos.FindAsync(id);
        if (todo is null) return NotFound();

        _db.Todos.Remove(todo);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
