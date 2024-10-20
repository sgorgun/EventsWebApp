﻿using EventsWebApp.Application.Validators;
using FluentValidation;

namespace EventsWebApp.Application.SocialEvents.Commands
{
    public class UpdateSocialEventValidator : AddUpdateSocialEventValidator<UpdateSocialEventCommand>
    {
        public UpdateSocialEventValidator() : base()
        {
            RuleFor(socialEvent => socialEvent.Id)
                .NotEmpty();
        }
    }
}
