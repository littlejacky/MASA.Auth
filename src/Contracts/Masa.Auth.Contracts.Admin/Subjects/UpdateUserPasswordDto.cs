﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Subjects;

public class UpdateUserPasswordDto
{
    public Guid UserId { get; set; }

    public string Password { get; set; } = "";

    public UpdateUserPasswordDto()
    {
    }

    public UpdateUserPasswordDto(Guid userId, string password)
    {
        UserId = userId;
        Password = password;
    }
}
