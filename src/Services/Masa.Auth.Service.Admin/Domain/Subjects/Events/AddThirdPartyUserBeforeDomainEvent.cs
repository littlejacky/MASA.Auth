﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Domain.Subjects.Events;

public record AddThirdPartyUserBeforeDomainEvent(UpsertUserModel User) : Event
{
    public UserModel Result { get; set; } = new();
}
