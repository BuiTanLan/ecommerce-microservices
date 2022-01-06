using Identity.Api.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class IdentityController : ControllerBase
{
    public IdentityController(UserManager<ApplicationIdentityUser> userManager)
    {
        _userManager = userManager;
    }

    private readonly UserManager<ApplicationIdentityUser> _userManager;

    [HttpGet]
    public IActionResult Get()
    {
        return new JsonResult(from c in User.Claims select new { c.Type, c.Value });
    }

    [Authorize(Roles = "user")]
    [HttpGet("user")]
    public IActionResult GetBasic()
    {
        return Ok(new { Role = "Basic" });
    }

    [Authorize(Roles = "admin")]
    [HttpGet("admin")]
    public IActionResult GetAdmin()
    {
        return Ok(new { Role = "admin" });
    }


    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] RegisterIdentityUserDto userDto)
    {
        var user = new ApplicationIdentityUser
        {
            UserName = userDto.Username,
            Email = userDto.Email
        };

        var result = await _userManager.CreateAsync(user, userDto.Password);

        if (result.Succeeded)
        {
            return Ok();
        }

        return BadRequest(result.Errors);
    }
}