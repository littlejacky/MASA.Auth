﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Application.Organizations.Queries;

public record DepartmentSelectQuery(string Name) : Query<List<DepartmentSelectDto>>
{
    public override List<DepartmentSelectDto> Result { get; set; } = new();
}
