﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.ApiGateways.Caller.Services.Projects;

public class ProjectService : ServiceBase
{
    protected override string BaseUrl { get; set; }

    internal ProjectService(ICaller caller) : base(caller)
    {
        BaseUrl = "api/project";
    }

    public async Task<List<ProjectDto>> GetListAsync()
    {
        return await GetAsync<List<ProjectDto>>($"GetList");
    }

    public async Task<List<ProjectDto>> GetUIAndMenusAsync()
    {
        return await GetAsync<List<ProjectDto>>($"GetUIAndMenus");
    }
}
