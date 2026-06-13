using System.Security.Claims;
using Application.Users.UseCases.Commands;
using MediatR;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.Circuits;

namespace ChatRoomApp.Services;

public class UserOnlineCircuitHandler : CircuitHandler
{
    private readonly AuthenticationStateProvider _authenticationStateProvider;
    private readonly IMediator _mediator;
    private readonly ILogger<UserOnlineCircuitHandler> _logger;
    private int _userId;

    public UserOnlineCircuitHandler(
        AuthenticationStateProvider authenticationStateProvider,
        IMediator mediator,
        ILogger<UserOnlineCircuitHandler> logger)
    {
        _authenticationStateProvider = authenticationStateProvider;
        _mediator = mediator;
        _logger = logger;
    }

    public override async Task OnCircuitOpenedAsync(Circuit circuit, CancellationToken cancellationToken)
    {
        try
        {
            var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
            var userIdClaim = authState.User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
            {
                _userId = userId;
                await _mediator.Send(new SetOnlineOrOfflineCommand { UserId = userId, IsOnline = true }, cancellationToken);
                _logger.LogInformation("User {UserId} set online via circuit open", userId);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to set user online on circuit open");
        }

        await base.OnCircuitOpenedAsync(circuit, cancellationToken);
    }

    public override async Task OnCircuitClosedAsync(Circuit circuit, CancellationToken cancellationToken)
    {
        if (_userId != 0)
        {
            try
            {
                await _mediator.Send(new SetOnlineOrOfflineCommand { UserId = _userId, IsOnline = false }, cancellationToken);
                _logger.LogInformation("User {UserId} set offline via circuit close", _userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to set user offline on circuit close");
            }
        }

        await base.OnCircuitClosedAsync(circuit, cancellationToken);
    }
}
