﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Sso;

public class ApiScopeDetailDto : ApiScopeDto
{
    public List<Guid> UserClaims { get; set; } = new();

    public Dictionary<string, string> Properties { get; set; } = new();

    public ApiScopeDetailDto()
    {
    }

    public ApiScopeDetailDto(Guid id, bool enabled, string name, string displayName, string description, bool required, bool emphasize, bool showInDiscoveryDocument, List<Guid> userClaims, Dictionary<string, string> properties) : base(id, enabled, name, displayName, description, required, emphasize, showInDiscoveryDocument)
    {
        UserClaims = userClaims;
        Properties = properties;
    }

    public static implicit operator UpdateApiScopeDto(ApiScopeDetailDto apiScope)
    {
        return new UpdateApiScopeDto(apiScope.Id, apiScope.Enabled, apiScope.Name, apiScope.DisplayName, apiScope.Description, apiScope.Required, apiScope.Emphasize, apiScope.ShowInDiscoveryDocument, apiScope.UserClaims, apiScope.Properties);
    }
}

