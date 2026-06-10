using Application.Users.UseCases.Queries;
using MediatR;

namespace ChatRoomApp.Endpoints;

public static class UsersEndpoints
{
    public static RouteGroupBuilder MapUsersEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/users").WithTags("Users").RequireAuthorization();

        group.MapGet("/{userId:int}", GetByIdAsync);
        group.MapGet("/search", SearchAsync);

        return group;
    }

    private static async Task<IResult> GetByIdAsync(int userId, IMediator mediator)
    {
        var user = await mediator.Send(new GetUserByIdQuery { UserId = userId });
        return user is null ? Results.NotFound() : Results.Ok(user);
    }

    private static async Task<IResult> SearchAsync(string q, IMediator mediator)
    {
        var user = await mediator.Send(new GetUserByEmailQuery { Email = q })
                    ?? await mediator.Send(new GetUserByUserNameQuery { UserName = q });
        return user is null ? Results.NotFound() : Results.Ok(user);
    }
}
