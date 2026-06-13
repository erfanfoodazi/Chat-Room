using Application.Users.UseCases.Commands;
using Application.Users.UseCases.Queries;
using ChatRoomApp.Models.Auth;
using Domain.Entities;
using MediatR;
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
            .RequireAuthorization();

        return group;
    }

    private static async Task<IResult> LoginAsync(
        LoginRequest request,
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        IMediator mediator)
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

        // Create cookie session
        await signInManager.SignInAsync(user, isPersistent: true);

        return Results.Ok(new AuthResponse
        {
            UserId = user.Id,
            UserName = user.UserName ?? string.Empty,
            Email = user.Email ?? string.Empty,
            FullName = user.FullName,
        });
    }

    private static async Task<IResult> RegisterAsync(
        RegisterRequest request,
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        IMediator mediator)
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
            
            // Auto-login after registration
            await signInManager.SignInAsync(user, isPersistent: true);

            return Results.Ok(new AuthResponse
            {
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
        IMediator mediator)
    {
        var userId = httpContext.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (userId is null)
            return Results.Unauthorized();

        var user = await mediator.Send(new GetUserByIdQuery { UserId = int.Parse(userId) });
        if (user is null)
            return Results.NotFound();

        return Results.Ok(user);
    }
}
