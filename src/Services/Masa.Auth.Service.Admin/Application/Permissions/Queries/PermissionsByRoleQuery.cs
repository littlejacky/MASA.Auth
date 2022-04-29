﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Application.Permissions.Queries;

public record PermissionsByRoleQuery(List<Guid> Roles) : Query<List<Guid>>
{
    public override List<Guid> Result { get; set; } = new();
}
