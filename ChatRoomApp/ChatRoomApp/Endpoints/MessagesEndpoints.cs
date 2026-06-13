using Application.Messages.UseCases.Commands;
using Application.Messages.UseCases.Queries;
using MediatR;

namespace ChatRoomApp.Endpoints;

public static class MessagesEndpoints
{
    public static RouteGroupBuilder MapMessagesEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/messages").WithTags("Messages").RequireAuthorization();

        group.MapGet("/", GetMessagesAsync);
        group.MapPut("/{messageId:int}", EditMessageAsync);
        group.MapDelete("/{messageId:int}", DeleteMessageAsync);

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

    private static async Task<IResult> EditMessageAsync(int messageId, EditMessageRequest request, IMediator mediator)
    {
        var msg = await mediator.Send(new UpdateMessageCommand
        {
            MessageId = messageId,
            Text = request.Text,
            SenderId = request.SenderId,
        });
        return msg is null ? Results.Forbid() : Results.Ok(msg);
    }

    private static async Task<IResult> DeleteMessageAsync(int messageId, int senderId, IMediator mediator)
    {
        var success = await mediator.Send(new DeleteMessageCommand
        {
            MessageId = messageId,
            SenderId = senderId,
        });
        return success ? Results.Ok() : Results.Forbid();
    }

    private record EditMessageRequest(string Text, int SenderId);
}
