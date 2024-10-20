﻿using EventsWebApp.Application.Interfaces.UseCases;

namespace EventsWebApp.Application.Users.Queries
{
    public record GetRoleByTokenQuery(string AccessToken) : IQuery<string?>;
}
