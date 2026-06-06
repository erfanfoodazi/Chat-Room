using Application.Users.UseCases.Commands;
using Application.Users.UseCases.Queries;
using ChatRoomApp.Models.Auth;
using ChatRoomApp.Services;
using Domain.Entities;
using MediatR;
using ChatRoomApp.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ChatRoomApp.Endpoints;

public static class AuthEndpoints
{
    public static RouteGroupBuilder MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/auth").WithTags("Auth");

        group.MapPost("/login", LoginAsync).AllowAnonymous();
        group.MapPost("/register", RegisterAsync).AllowAnonymous();
        group.MapGet("/me", GetCurrentUserAsync)
            .RequireAuthorization(new AuthorizeAttribute
            {
                AuthenticationSchemes = JwtAuthenticationDefaults.AuthenticationScheme
            });

        return group;
    }

    private static async Task<IResult> LoginAsync(
        LoginRequest request,
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        IMediator mediator,
        ITokenService tokenService)
    {
        var userDto = await mediator.Send(new GetUserByEmailQuery { Email = request.Email });
        if (userDto is null)
            return Results.Unauthorized();

        var user = await userManager.FindByIdAsync(userDto.Id.ToString());
        if (user is null)
            return Results.Unauthorized();

        var result = await signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: true);
        if (!result.Succeeded)
            return result.IsLockedOut
                ? Results.Problem("Account is locked. Try again later.", statusCode: StatusCodes.Status423Locked)
                : Results.Unauthorized();

        var token = await tokenService.GenerateTokenAsync(user);

        return Results.Ok(new AuthResponse
        {
            Token = token,
            ExpiresAt = tokenService.GetExpiry(),
            UserId = user.Id,
            UserName = user.UserName ?? string.Empty,
            Email = user.Email ?? string.Empty,
            FullName = user.FullName,
        });
    }

    private static async Task<IResult> RegisterAsync(
        RegisterRequest request,
        UserManager<User> userManager,
        IMediator mediator,
        ITokenService tokenService)
    {
        try
        {
            var userId = await mediator.Send(new AddNewUserCommand
            {
                FullName = request.FullName,
                UserName = request.UserName,
                Email = request.Email,
                Password = request.Password,
            });

            var user = await userManager.FindByIdAsync(userId.ToString());
            if (user is null)
                return Results.Problem("User was created but could not be loaded.", statusCode: StatusCodes.Status500InternalServerError);

            await userManager.AddToRoleAsync(user, "User");

            var token = await tokenService.GenerateTokenAsync(user);

            return Results.Ok(new AuthResponse
            {
                Token = token,
                ExpiresAt = tokenService.GetExpiry(),
                UserId = user.Id,
                UserName = user.UserName ?? string.Empty,
                Email = user.Email ?? string.Empty,
                FullName = user.FullName,
            });
        }
        catch (Exception ex)
        {
            return Results.BadRequest(new { message = ex.Message });
        }
    }

    private static async Task<IResult> GetCurrentUserAsync(
        HttpContext httpContext,
        UserManager<User> userManager)
    {
        var userId = httpContext.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (userId is null)
            return Results.Unauthorized();

        var user = await userManager.FindByIdAsync(userId);
        if (user is null)
            return Results.NotFound();

        return Results.Ok(new
        {
            user.Id,
            user.UserName,
            user.Email,
            user.FullName,
            user.IsOnline,
            user.LastSeen,
        });
    }
}
