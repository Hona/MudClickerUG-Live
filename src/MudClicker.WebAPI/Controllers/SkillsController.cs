using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MudClicker.Application;
using MudClicker.Domain.Models;

namespace MudClicker.WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController()]
[Authorize]
public class SkillsController : Controller
{
    private readonly CodeService _codeService;

    public SkillsController(CodeService codeService)
    {
        _codeService = codeService;
    }

    [HttpPost(nameof(SquashBug))]
    public async Task<IActionResult> SquashBug()
    {
        await _codeService.SquashBugAsync();
        return Ok();
    }
    
    [HttpGet("leaderboard")]
    [ProducesResponseType(typeof(List<Player>), 200)]
    public async Task<IActionResult> GetLeaderboard()
    {
        var output = await _codeService.GetLeaderboardAsync();
        return Ok(output);
    }
    
}