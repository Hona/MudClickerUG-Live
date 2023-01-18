using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace MudClicker.Application;

public class CurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }
    
    public string GetUserName()
    {
        return _httpContextAccessor.HttpContext?.User.Identity.Name;
    }

    public string GetUserId()
    {
        return _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new InvalidOperationException();
    }
}