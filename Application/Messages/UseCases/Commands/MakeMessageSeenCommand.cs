using Application.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Messages.UseCases.Commands
{
    public class MakeMessageSeenCommand : IRequest<bool>
    {
        public int MessageId { get; set; }
    }

    public class MakeMessageSeenCommandHandler : IRequestHandler<MakeMessageSeenCommand, bool>
    {
        private readonly IMessageRepository _messageRepository;
        public MakeMessageSeenCommandHandler(IMessageRepository messageRepository)
        {
            _messageRepository = messageRepository;
        }
        public async Task<bool> Handle(MakeMessageSeenCommand request, CancellationToken cancellationToken)
        {
            return await _messageRepository.MakeMessageSeenAsync(request.MessageId);
        }
    }
}
