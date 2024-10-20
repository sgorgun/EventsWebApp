﻿using EventsWebApp.Application.Helpers;
using EventsWebApp.Application.Interfaces;
using EventsWebApp.Application.Interfaces.UseCases;
using MediatR;

namespace EventsWebApp.Application.Attendees.Commands
{
    public class AddAttendeeWithTokenHandler : ICommandHandler<AddAttendeeWithTokenCommand, Guid>
    {
        private readonly IMediator _mediator;
        private readonly IJwtProvider _jwtProvider;
        public AddAttendeeWithTokenHandler(IMediator mediator, IJwtProvider jwtProvider)
        {
            _mediator = mediator;
            _jwtProvider = jwtProvider;
        }

        public async Task<Guid> Handle(AddAttendeeWithTokenCommand request, CancellationToken cancellationToken)
        {
            Guid userId = TokenHelper.CheckToken(request.AccessToken, _jwtProvider);
            return await _mediator.Send(new AddAttendeeCommand(request.Request, request.EventId, userId), cancellationToken);
        }
    }
}
