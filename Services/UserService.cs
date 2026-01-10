using System.Text.Json;
using Tutor.Models;

namespace Tutor.Services;

public class UserService
{
    private const string UserKey = "ACTIVE_USER";
    private User? currentUser;

    public async Task<User> GetCurrentUserAsync()
    {
        if (currentUser != null) return currentUser;

        try
        {
            var json = await SecureStorage.GetAsync(UserKey);
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
        await SecureStorage.SetAsync(UserKey, json);
    }
}
