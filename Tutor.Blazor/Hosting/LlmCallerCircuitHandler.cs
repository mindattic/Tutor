using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.Circuits;
using MindAttic.Authentication;
using Tutor.Core.Services;
using Tutor.Shared.Services;

namespace Tutor.Blazor.Hosting;

/// <summary>
/// Gives the singleton LLM services the signed-in user without threading a user id through every call
/// (TUT-T59; per-user state proper is plan item 0.3). Around every inbound activity on a circuit - UI events,
/// JS interop, component updates - it sets <see cref="LlmCaller.Current"/> from the circuit's
/// <see cref="AuthenticationState"/>, so <see cref="LlmUsageLimiter"/> counts the request against that user.
/// Async work started from the activity carries the caller with it.
/// </summary>
public sealed class LlmCallerCircuitHandler(
    AuthenticationStateProvider authState,
    IAuthorizationService authorization,
    TutorSettingsSource settings) : CircuitHandler
{
    public override Func<CircuitInboundActivityContext, Task> CreateInboundActivityHandler(
        Func<CircuitInboundActivityContext, Task> next) =>
        async context =>
        {
            // Set inside this async lambda, the caller flows into next() and is undone when it returns.
            _ = LlmCaller.Use(await ResolveCallerAsync());
            await next(context);
        };

    private async Task<LlmCallerInfo?> ResolveCallerAsync()
    {
        var user = (await authState.GetAuthenticationStateAsync()).User;
        if (user.Identity?.IsAuthenticated != true) return null;

        var id = user.FindFirst(MaClaims.UserId)?.Value ?? user.Identity.Name;
        if (string.IsNullOrWhiteSpace(id)) return null;

        var exempt = settings.Current.LlmLimitsExemptAuthors
            && (await authorization.AuthorizeAsync(user, resource: null, TutorPolicies.Author)).Succeeded;
        return new LlmCallerInfo(id, exempt);
    }
}
