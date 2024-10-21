﻿using EventsWebApp.Application.Validators;
using FluentValidation;

namespace EventsWebApp.Application.Users.Commands
{
    public class RegisterUserValidator : LoginRegisterUserValidator<RegisterUserCommand>
    {
        public RegisterUserValidator() :base(){
            RuleFor(user => user.Username)
                .NotEmpty()
                .MaximumLength(100);
        }
    }
}
