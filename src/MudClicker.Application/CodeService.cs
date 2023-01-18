using MudClicker.Domain.Models;
using MudClicker.Infrastructure;

namespace MudClicker.Application;

public class CodeService
{
    private readonly GenericRepository _genericRepository;
    private readonly CurrentUserService _currentUserService;

    public CodeService(GenericRepository genericRepository, CurrentUserService currentUserService)
    {
        _genericRepository = genericRepository;
        _currentUserService = currentUserService;
    }

    public async Task SquashBugAsync()
    {
        _currentUserService.GetUserId();
        
        var user = await _genericRepository.GetItemAsync<Player>(new Player()
        {
            Id = _currentUserService.GetUserId()
        }, x => x.Id, x => x.Id);

        user ??= new Player
        {
            Id = _currentUserService.GetUserId(),
            DisplayName = _currentUserService.GetUserName(),
            Skills = new PlayerSkills
            {
                { Skill.Code, 0 }
            }
        };

        user.Skills[Skill.Code] += 1;

        await _genericRepository.CreateOrUpdateItemAsync(user);
    }

    public async Task<List<Player>> GetLeaderboardAsync()
    {
        var output = await _genericRepository.GetItemListAsync<Player>();
        return output.OrderByDescending(x => x.Skills[Skill.Code]).ToList();
    }
}