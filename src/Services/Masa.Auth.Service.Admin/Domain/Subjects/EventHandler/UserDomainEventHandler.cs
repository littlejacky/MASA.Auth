﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Domain.Subjects.EventHandler;

public class UserDomainEventHandler
{
    readonly IAutoCompleteClient _autoCompleteClient;

    public UserDomainEventHandler(IAutoCompleteClient autoCompleteClient)
    {
        _autoCompleteClient = autoCompleteClient;
    }

    [EventHandler]
    public async Task SetUserAsync(SetUserDomainEvent userEvent)
    {
        var user = userEvent.Users;
        var response = await _autoCompleteClient.SetAsync<UserSelectDto, Guid>(userEvent.Users.Select(user => new UserSelectDto(user.Id, user.Name, user.DisplayName, user.Account, user.PhoneNumber, user.Email, user.Avatar)));
    }

    [EventHandler]
    public async Task RemoveUserAsync(RemoveUserDomainEvent userEvent)
    {
        var response = await _autoCompleteClient.DeleteAsync(userEvent.userIds);
    }
}
