using Application.GroupChats.UseCases.Commands;
using Application.GroupChats.UseCases.DTOs;
using Application.GroupChats.UseCases.Queries;
using MediatR;

namespace ChatRoomApp.Endpoints;

public static class GroupChatsEndpoints
{
    public static RouteGroupBuilder MapGroupChatsEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/chats/groups").WithTags("Group Chats").RequireAuthorization();

        group.MapGet("/", GetByUserIdAsync);
        group.MapGet("/{groupId:int}", GetByIdAsync);
        group.MapPost("/", CreateAsync);

        return group;
    }

    private static async Task<IResult> GetByUserIdAsync(int userId, IMediator mediator)
    {
        var chats = await mediator.Send(new GetAllGroupByUserIdQuery { UserId = userId });
        return Results.Ok(chats);
    }

    private static async Task<IResult> GetByIdAsync(int groupId, IMediator mediator)
    {
        var chat = await mediator.Send(new GetGroupChatByIdQuery { GroupId = groupId });
        return chat is null ? Results.NotFound() : Results.Ok(chat);
    }

    private static async Task<IResult> CreateAsync(CreateGroupChatRequest request, IMediator mediator)
    {
        var group = await mediator.Send(new CreateGroupChatCommand
        {
            Name = request.Name,
            Description = request.Description,
            OwnerId = request.OwnerId,
        });
        return group is null ? Results.BadRequest() : Results.Ok(group);
    }

    private record CreateGroupChatRequest(string Name, string? Description, int OwnerId);
}
