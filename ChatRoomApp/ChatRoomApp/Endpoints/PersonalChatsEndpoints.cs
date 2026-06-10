using Application.PersonalChats.UseCases;
using Application.PersonalChats.UseCases.Commands;
using Application.PersonalChats.UseCases.Queries;
using MediatR;

namespace ChatRoomApp.Endpoints;

public static class PersonalChatsEndpoints
{
    public static RouteGroupBuilder MapPersonalChatsEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/chats/personal").WithTags("Personal Chats").RequireAuthorization();

        group.MapGet("/", GetByUserIdAsync);
        group.MapGet("/{chatId:int}", GetByIdAsync);
        group.MapPost("/", CreateAsync);

        return group;
    }

    private static async Task<IResult> GetByUserIdAsync(int userId, IMediator mediator)
    {
        var chats = await mediator.Send(new GetPersonalChatsByUserIdQuery { UserId = userId });
        return Results.Ok(chats);
    }

    private static async Task<IResult> GetByIdAsync(int chatId, IMediator mediator)
    {
        var chat = await mediator.Send(new GetPersonalChatByIdQuery { PersonalId = chatId });
        return chat is null ? Results.NotFound() : Results.Ok(chat);
    }

    private static async Task<IResult> CreateAsync(CreatePersonalChatRequest request, IMediator mediator)
    {
        var chat = await mediator.Send(new CreatePersonalChatCommand
        {
            UserOneId = request.UserOneId,
            UserTwoId = request.UserTwoId,
        });
        return chat is null ? Results.BadRequest() : Results.Ok(chat);
    }

    private record CreatePersonalChatRequest(int UserOneId, int UserTwoId);
}
