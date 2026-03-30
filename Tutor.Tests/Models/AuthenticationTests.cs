using Tutor.Core.Models;

namespace Tutor.Tests.Models;

public class AuthenticationTests
{
    [Fact]
    public void UserProfile_DefaultRole_IsStudent()
    {
        var profile = new UserProfile();
        Assert.Single(profile.Roles);
        Assert.Equal(UserRole.Student, profile.Roles[0]);
    }

    [Fact]
    public void UserProfile_HasRole_ChecksCorrectly()
    {
        var profile = new UserProfile { Roles = [UserRole.Student, UserRole.Admin] };
        Assert.True(profile.HasRole(UserRole.Admin));
        Assert.True(profile.HasRole(UserRole.Student));
    }

    [Fact]
    public void UserProfile_IsAdmin_TrueForAdminRole()
    {
        var admin = new UserProfile { Roles = [UserRole.Admin] };
        Assert.True(admin.IsAdmin);
    }

    [Fact]
    public void UserProfile_IsAdmin_FalseForStudentOnly()
    {
        var student = new UserProfile();
        Assert.False(student.IsAdmin);
    }

    [Fact]
    public void AuthResult_Succeeded_SetsProperties()
    {
        var user = new UserProfile { Username = "alice" };
        var result = AuthResult.Succeeded(user);
        Assert.True(result.Success);
        Assert.NotNull(result.User);
        Assert.Equal("alice", result.User.Username);
        Assert.Null(result.ErrorMessage);
    }

    [Fact]
    public void AuthResult_Failed_SetsErrorMessage()
    {
        var result = AuthResult.Failed("bad password");
        Assert.False(result.Success);
        Assert.Null(result.User);
        Assert.Equal("bad password", result.ErrorMessage);
    }

    [Fact]
    public void UserCredentials_DefaultValues()
    {
        var creds = new UserCredentials();
        Assert.Equal("", creds.Username);
        Assert.Equal("", creds.PasswordHash);
        Assert.Null(creds.LastLoginAt);
    }

    [Fact]
    public void UsersStore_DefaultValues()
    {
        var store = new UsersStore();
        Assert.Empty(store.Credentials);
        Assert.Empty(store.Profiles);
        Assert.Null(store.LastLoggedInUsername);
    }

    [Theory]
    [InlineData(UserRole.Student)]
    [InlineData(UserRole.Admin)]
    public void UserRole_Enum_ValidValues(UserRole role)
    {
        Assert.True(Enum.IsDefined(role));
    }

    [Theory]
    [InlineData(SectionStatus.NotStarted)]
    [InlineData(SectionStatus.Visited)]
    [InlineData(SectionStatus.Read)]
    [InlineData(SectionStatus.Complete)]
    public void SectionStatus_Enum_ValidValues(SectionStatus status)
    {
        Assert.True(Enum.IsDefined(status));
    }
}
