using Application.Interfaces;
using Application.Messages.UseCases.Commands;
using Application.Messages.UseCases.Dto;
using Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.GroupChats.UseCases.Commands
{
    public class SendMessageToGroupCommand : IRequest<MessageDto>
    {
        public int GroupId { get; set; }
        public string Text { get; set; } = string.Empty;
        public int SenderId {  get; set; }
        public int? ReplyToMessageId { get; set; }
    }

    public class SendMessageToGroupCommandHandler : IRequestHandler<SendMessageToGroupCommand, MessageDto>
    {
        private readonly IGroupChatRepository _groupChatRepository;
        private readonly IMediator _mediator;

        public SendMessageToGroupCommandHandler(IGroupChatRepository groupChatRepository, IMediator mediator)
        {
            _groupChatRepository = groupChatRepository;
            _mediator = mediator;
        }
        public async Task<MessageDto> Handle(SendMessageToGroupCommand request, CancellationToken cancellationToken)
        {
            var group = await _groupChatRepository.GetGroupChatByGroupId(request.GroupId);
            if (group == null)
                return null;

            var message = await _mediator.Send(new CreateMessageCommand
            {
                Text = request.Text,
                GroupChatId = request.GroupId,
                SenderId = request.SenderId,
                ReplyToMessageId = request.ReplyToMessageId,
            }, cancellationToken);

            return message;

        }
    }
}
