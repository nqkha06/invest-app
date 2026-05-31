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
                Username = user.Username,
                Email = user.Email
            })
            .ToListAsync(ct);

        return Ok(users);
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<UserResponse>> GetById(long id, CancellationToken ct)
    {
        var user = await _db.Users
            .AsNoTracking()
            .Where(current => current.Id == id)
            .Select(current => new UserResponse
            {
                Id = current.Id,
                Username = current.Username,
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
            Username = request.Username.Trim(),
            Email = request.Email.Trim()
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync(ct);

        var response = new UserResponse
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email
        };

        return CreatedAtAction(nameof(GetById), new { id = user.Id }, response);
    }

    [HttpPut("{id:long}")]
    public async Task<ActionResult<UserResponse>> Update(
        long id,
        [FromBody] UpdateUserRequest request,
        CancellationToken ct
    )
    {
        var user = await _db.Users.FirstOrDefaultAsync(current => current.Id == id, ct);

        if (user is null)
        {
            return NotFound();
        }

        user.Username = request.Username.Trim();
        user.Email = request.Email.Trim();

        await _db.SaveChangesAsync(ct);

        var response = new UserResponse
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email
        };

        return Ok(response);
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id, CancellationToken ct)
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
