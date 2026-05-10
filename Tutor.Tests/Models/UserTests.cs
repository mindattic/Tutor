using Tutor.Core.Models;

namespace Tutor.Tests.Models;

public class UserTests
{
    [Test]
    public void RequiredProperties_RoundTrip()
    {
        var user = new User { Id = "u1", Name = "Alice" };

        Assert.That(user.Id, Is.EqualTo("u1"));
        Assert.That(user.Name, Is.EqualTo("Alice"));
        Assert.That(user.Email, Is.Null);
        Assert.That(user.Roles, Has.Count.EqualTo(1));
        Assert.That(user.Roles[0], Is.EqualTo(UserRole.Student));
    }

    [Test]
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

        Assert.That(user.Id, Is.EqualTo("p1"));
        Assert.That(user.Name, Is.EqualTo("Bob"));
        Assert.That(user.Email, Is.EqualTo("bob@example.com"));
        Assert.That(user.Roles, Has.Count.EqualTo(2));
        Assert.That(user.Roles, Does.Contain(UserRole.Admin));
    }

    [Test]
    public void Equality_OnSharedReferenceFields_RecordSemantics()
    {
        // User is a record but Roles is a List<T>, which uses reference equality.
        // Two records sharing the same list reference compare equal; two records
        // each with their own default list do not.
        var roles = new List<UserRole> { UserRole.Student };
        var a = new User { Id = "x", Name = "y", Roles = roles };
        var b = new User { Id = "x", Name = "y", Roles = roles };
        Assert.That(b, Is.EqualTo(a));
    }
}
