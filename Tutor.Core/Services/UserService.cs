using System.Text.Json;
using Tutor.Core.Models;
using Tutor.Core.Services.Abstractions;

namespace Tutor.Core.Services;

public class UserService
{
    private const string UserKey = "ACTIVE_USER";
    private readonly ISecurePreferences _securePreferences;
    private User? currentUser;

    public UserService(ISecurePreferences securePreferences)
    {
        _securePreferences = securePreferences;
    }

    public async Task<User> GetCurrentUserAsync()
    {
        if (currentUser != null) return currentUser;

        try
        {
            var json = await _securePreferences.GetAsync(UserKey);
            if (!string.IsNullOrEmpty(json))
            {
                currentUser = JsonSerializer.Deserialize<User>(json);
                if (currentUser != null) return currentUser;
            }
        }
        catch { }

        // Create a default user
        currentUser = new User { Id = "default_user", Name = "Default User" };
        await SaveCurrentUserAsync(currentUser);
        return currentUser;
    }

    public async Task SaveCurrentUserAsync(User user)
    {
        currentUser = user;
        var json = JsonSerializer.Serialize(user);
        await _securePreferences.SetAsync(UserKey, json);
    }
}
