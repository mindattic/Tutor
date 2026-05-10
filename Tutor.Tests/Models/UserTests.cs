using Tutor.Core.Models;

namespace Tutor.Tests.Models;

public class UserTests
{
    [Fact]
    public void RequiredProperties_RoundTrip()
    {
        var user = new User { Id = "u1", Name = "Alice" };

        Assert.Equal("u1", user.Id);
        Assert.Equal("Alice", user.Name);
        Assert.Null(user.Email);
        Assert.Single(user.Roles);
        Assert.Equal(UserRole.Student, user.Roles[0]);
    }

    [Fact]
    public void FromProfile_CopiesIdNameEmailRoles()
    {
        var profile = new UserProfile
        {
            Id = "p1",
            DisplayName = "Bob",
            Email = "bob@example.com",
            Roles = [UserRole.Student, UserRole.Admin]
        };

        var user = User.FromProfile(profile);

        Assert.Equal("p1", user.Id);
        Assert.Equal("Bob", user.Name);
        Assert.Equal("bob@example.com", user.Email);
        Assert.Equal(2, user.Roles.Count);
        Assert.Contains(UserRole.Admin, user.Roles);
    }

    [Fact]
    public void Equality_OnSharedReferenceFields_RecordSemantics()
    {
        // User is a record but Roles is a List<T>, which uses reference equality.
        // Two records sharing the same list reference compare equal; two records
        // each with their own default list do not.
        var roles = new List<UserRole> { UserRole.Student };
        var a = new User { Id = "x", Name = "y", Roles = roles };
        var b = new User { Id = "x", Name = "y", Roles = roles };
        Assert.Equal(a, b);
    }
}
