﻿namespace Masa.Auth.Service.Admin.Application.Sso.Queries;

public record IdentityResourcesQuery(int Page, int PageSize, string Search) : Query<PaginationDto<IdentityResourceDto>>
{
    public override PaginationDto<IdentityResourceDto> Result { get; set; } = new();
}