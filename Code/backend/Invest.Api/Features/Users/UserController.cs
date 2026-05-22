using Invest.Api.Data;
using Invest.Api.Features.Users.Dtos;
using UserEntity = Invest.Api.Entities.User;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Invest.Api.Features.Users;

[ApiController]
[Route("api/v1/users")]
public class UserController : ControllerBase
{
    private readonly AppDbContext _db;

    public UserController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<ActionResult<List<UserResponse>>> GetAll(CancellationToken ct)
    {
        var users = await _db.Users
            .AsNoTracking()
            .Select(user => new UserResponse
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email
            })
            .ToListAsync(ct);

        return Ok(users);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<UserResponse>> GetById(int id, CancellationToken ct)
    {
        var user = await _db.Users
            .AsNoTracking()
            .Where(current => current.Id == id)
            .Select(current => new UserResponse
            {
                Id = current.Id,
                Name = current.Name,
                Email = current.Email
            })
            .FirstOrDefaultAsync(ct);

        if (user is null)
        {
            return NotFound();
        }

        return Ok(user);
    }

    [HttpPost]
    public async Task<ActionResult<UserResponse>> Create(
        [FromBody] CreateUserRequest request,
        CancellationToken ct
    )
    {
        var user = new UserEntity
        {
            Name = request.Name.Trim(),
            Email = request.Email.Trim()
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync(ct);

        var response = new UserResponse
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email
        };

        return CreatedAtAction(nameof(GetById), new { id = user.Id }, response);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<UserResponse>> Update(
        int id,
        [FromBody] UpdateUserRequest request,
        CancellationToken ct
    )
    {
        var user = await _db.Users.FirstOrDefaultAsync(current => current.Id == id, ct);

        if (user is null)
        {
            return NotFound();
        }

        user.Name = request.Name.Trim();
        user.Email = request.Email.Trim();

        await _db.SaveChangesAsync(ct);

        var response = new UserResponse
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email
        };

        return Ok(response);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var user = await _db.Users.FirstOrDefaultAsync(current => current.Id == id, ct);

        if (user is null)
        {
            return NotFound();
        }

        _db.Users.Remove(user);
        await _db.SaveChangesAsync(ct);

        return NoContent();
    }
}
