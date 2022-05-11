﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Domain.Sso.Repositories;

public interface IApiScopeRepository : IRepository<ApiScope, int>
{
    Task<ApiScope?> GetDetailAsync(int id);
}