using Application.Messages.UseCases.Queries;
using MediatR;

namespace ChatRoomApp.Endpoints;

public static class MessagesEndpoints
{
    public static RouteGroupBuilder MapMessagesEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/messages").WithTags("Messages").RequireAuthorization();

        group.MapGet("/", GetMessagesAsync);

        return group;
    }

    private static async Task<IResult> GetMessagesAsync(int? personalChatId, int? groupChatId, IMediator mediator)
    {
        if (personalChatId.HasValue)
        {
            var msgs = await mediator.Send(new GetMessagesByPersonalChatIdQuery { PersonalId = personalChatId.Value });
            return Results.Ok(msgs);
        }
        if (groupChatId.HasValue)
        {
            var msgs = await mediator.Send(new GetMessagesByGroupChatIdQuery { GroupId = groupChatId.Value });
            return Results.Ok(msgs);
        }
        return Results.BadRequest("Provide personalChatId or groupChatId");
    }
}
