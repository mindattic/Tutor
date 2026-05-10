using Tutor.Core.Models;

namespace Tutor.Tests.Models;

public class AuthenticationTests
{
    [Test]
    public void UserProfile_DefaultRole_IsStudent()
    {
        var profile = new UserProfile();
        Assert.That(profile.Roles, Has.Count.EqualTo(1));
        Assert.That(profile.Roles[0], Is.EqualTo(UserRole.Student));
    }

    [Test]
    public void UserProfile_HasRole_ChecksCorrectly()
    {
        var profile = new UserProfile { Roles = [UserRole.Student, UserRole.Admin] };
        Assert.That(profile.HasRole(UserRole.Admin), Is.True);
        Assert.That(profile.HasRole(UserRole.Student), Is.True);
    }

    [Test]
    public void UserProfile_IsAdmin_TrueForAdminRole()
    {
        var admin = new UserProfile { Roles = [UserRole.Admin] };
        Assert.That(admin.IsAdmin, Is.True);
    }

    [Test]
    public void UserProfile_IsAdmin_FalseForStudentOnly()
    {
        var student = new UserProfile();
        Assert.That(student.IsAdmin, Is.False);
    }

    [Test]
    public void AuthResult_Succeeded_SetsProperties()
    {
        var user = new UserProfile { Username = "alice" };
        var result = AuthResult.Succeeded(user);
        Assert.That(result.Success, Is.True);
        Assert.That(result.User, Is.Not.Null);
        Assert.That(result.User!.Username, Is.EqualTo("alice"));
        Assert.That(result.ErrorMessage, Is.Null);
    }

    [Test]
    public void AuthResult_Failed_SetsErrorMessage()
    {
        var result = AuthResult.Failed("bad password");
        Assert.That(result.Success, Is.False);
        Assert.That(result.User, Is.Null);
        Assert.That(result.ErrorMessage, Is.EqualTo("bad password"));
    }

    [Test]
    public void UserCredentials_DefaultValues()
    {
        var creds = new UserCredentials();
        Assert.That(creds.Username, Is.EqualTo(""));
        Assert.That(creds.PasswordHash, Is.EqualTo(""));
        Assert.That(creds.LastLoginAt, Is.Null);
    }

    [Test]
    public void UsersStore_DefaultValues()
    {
        var store = new UsersStore();
        Assert.That(store.Credentials, Is.Empty);
        Assert.That(store.Profiles, Is.Empty);
        Assert.That(store.LastLoggedInUsername, Is.Null);
    }

    [TestCase(UserRole.Student)]
    [TestCase(UserRole.Admin)]
    public void UserRole_Enum_ValidValues(UserRole role)
    {
        Assert.That(Enum.IsDefined(role), Is.True);
    }

    [TestCase(SectionStatus.NotStarted)]
    [TestCase(SectionStatus.Visited)]
    [TestCase(SectionStatus.Read)]
    [TestCase(SectionStatus.Complete)]
    public void SectionStatus_Enum_ValidValues(SectionStatus status)
    {
        Assert.That(Enum.IsDefined(status), Is.True);
    }
}
