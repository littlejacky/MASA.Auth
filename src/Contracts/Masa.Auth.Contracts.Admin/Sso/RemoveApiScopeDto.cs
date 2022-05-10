﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Sso;

public class RemoveApiScopeDto
{
    public int Id { get; set; }

    public RemoveApiScopeDto(int id)
    {
        Id = id;
    }
}

