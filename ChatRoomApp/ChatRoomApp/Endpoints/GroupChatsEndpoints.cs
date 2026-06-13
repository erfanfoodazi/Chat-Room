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
        group.MapPut("/{groupId:int}", UpdateAsync);
        group.MapDelete("/{groupId:int}", DeleteAsync);
        group.MapPost("/{groupId:int}/members", AddMemberAsync);
        group.MapDelete("/{groupId:int}/leave", LeaveAsync);

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

    private static async Task<IResult> UpdateAsync(int groupId, UpdateGroupChatRequest request, IMediator mediator)
    {
        var success = await mediator.Send(new UpdateGroupChatCommand
        {
            GroupId = groupId,
            Name = request.Name,
            Description = request.Description ?? string.Empty,
            RequestedByUserId = request.RequestedByUserId,
        });
        return success ? Results.Ok() : Results.Forbid();
    }

    private static async Task<IResult> DeleteAsync(int groupId, int userId, IMediator mediator)
    {
        var success = await mediator.Send(new DeleteGroupChatCommand
        {
            GroupId = groupId,
            RequestedByUserId = userId,
        });
        return success ? Results.Ok() : Results.Forbid();
    }

    private static async Task<IResult> AddMemberAsync(int groupId, AddMemberRequest request, IMediator mediator)
    {
        var success = await mediator.Send(new AddUserToGroupCommand
        {
            GroupId = groupId,
            UserId = request.UserId,
            RequestedByUserId = request.RequestedByUserId,
        });
        return success ? Results.Ok() : Results.Forbid();
    }

    private static async Task<IResult> LeaveAsync(int groupId, int userId, IMediator mediator)
    {
        var success = await mediator.Send(new LeaveGroupCommand
        {
            GroupId = groupId,
            UserId = userId,
        });
        return success ? Results.Ok() : Results.BadRequest();
    }

    private record CreateGroupChatRequest(string Name, string? Description, int OwnerId);
    private record UpdateGroupChatRequest(string Name, string? Description, int RequestedByUserId);
    private record AddMemberRequest(int UserId, int RequestedByUserId);
}
