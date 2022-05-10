﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Application.Sso.Queries;

public record ApiScopeQuery(int Page, int PageSize, string Search) : Query<PaginationDto<ApiScopeDto>>
{
    public override PaginationDto<ApiScopeDto> Result { get; set; } = new();
}
