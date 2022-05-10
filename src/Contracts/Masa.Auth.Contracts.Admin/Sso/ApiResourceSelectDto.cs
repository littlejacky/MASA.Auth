﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Sso;

public class ApiResourceSelectDto
{
    public int Id { get; set; }

    public string Name { get; set; }

    public string DisplayName { get; set; }

    public string Description { get; set; }

    public ApiResourceSelectDto(int id, string name, string displayName, string description)
    {
        Id = id;
        Name = name;
        DisplayName = displayName;
        Description = description;
    }
}

