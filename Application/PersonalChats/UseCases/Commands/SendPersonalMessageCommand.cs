using Application.Interfaces;
using Application.Messages.UseCases.Commands;
using Application.Messages.UseCases.Dto;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.PersonalChats.UseCases.Commands
{
    public class SendPersonalMessageCommand : IRequest<MessageDto>
    {
        public string Text { get; set; } = string.Empty;
        public int SenderId { get; set; }
        public int ReceiverId { get; set; }
        public int PersonalChatId { get; set; }
        public int? ReplyToMessageId { get; set; }
    }

    public class SendPersonalMessageCommandHandler : IRequestHandler<SendPersonalMessageCommand, MessageDto>
    {
        private readonly IPersonalChatRepository _personalChatRepository;
        private readonly IMediator _mediator;
        public SendPersonalMessageCommandHandler(IPersonalChatRepository personalChatRepository, IMediator mediator)
        {
            _personalChatRepository = personalChatRepository;
            _mediator = mediator;
        }

        public async Task<MessageDto> Handle(SendPersonalMessageCommand request, CancellationToken cancellationToken)
        {
            var message = await _mediator.Send(new CreateMessageCommand
            {
                Text = request.Text,
                SenderId = request.SenderId,
                ReceiverId = request.ReceiverId,
                PersonalChatId = request.PersonalChatId,
                ReplyToMessageId = request.ReplyToMessageId,
            }, cancellationToken);

            return message;
        }
    }
}
