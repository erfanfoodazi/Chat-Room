using Application.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Messages.UseCases.Commands
{
    public class MakeMessageDeliveredCommand : IRequest<bool>
    {
        public int MessageId { get; set; }
    }

    public class MakeMessageDeliveredCommandHandler : IRequestHandler<MakeMessageDeliveredCommand, bool>
    {
        private readonly IMessageRepository _messageRepository;
        public MakeMessageDeliveredCommandHandler(IMessageRepository messageRepository)
        {
            _messageRepository = messageRepository;
        }
        public async Task<bool> Handle(MakeMessageDeliveredCommand request, CancellationToken cancellationToken)
        {
            return await _messageRepository.MakeMessageDeliveredAsync(request.MessageId);
        }
    }
}
