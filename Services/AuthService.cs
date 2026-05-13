using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace MuhasebeApp.Web.Services;

public class AuthService
{
    private readonly AuthenticationStateProvider _auth;

    public AuthService(AuthenticationStateProvider auth)
    {
        _auth = auth;
    }

    public async Task<string?> GetUserIdAsync()
    {
        var state = await _auth.GetAuthenticationStateAsync();
        return state.User.FindFirstValue(ClaimTypes.NameIdentifier);
    }

    public async Task<bool> IsAuthenticatedAsync()
    {
        var state = await _auth.GetAuthenticationStateAsync();
        return state.User.Identity?.IsAuthenticated ?? false;
    }

    public async Task<string?> GetUserNameAsync()
    {
        var state = await _auth.GetAuthenticationStateAsync();
        return state.User.Identity?.Name;
    }
}
