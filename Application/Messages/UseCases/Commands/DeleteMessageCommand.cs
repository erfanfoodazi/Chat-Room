using Application.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Messages.UseCases.Commands
{
    public class DeleteMessageCommand : IRequest<bool>
    {
        public int MessageId { get; set; }
    }

    public class DeleteMessageCommandHandler : IRequestHandler<DeleteMessageCommand, bool>
    {
        private readonly IMessageRepository _messageRepository;
        public DeleteMessageCommandHandler(IMessageRepository messageRepository)
        {
            _messageRepository = messageRepository;
        }
        public async Task<bool> Handle(DeleteMessageCommand request, CancellationToken cancellationToken)
        {
            return await _messageRepository.DeleteMessageAsync(request.MessageId);
        }
    }
}
