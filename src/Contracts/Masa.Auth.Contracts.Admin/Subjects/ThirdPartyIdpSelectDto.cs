﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Subjects;

public class ThirdPartyIdpSelectDto
{
    public Guid Id { get; set; }

    public string Name { get; set; } = "";

    public string DisplayName { get; set; } = "";

    public string ClientId { get; set; } = "";

    public string ClientSecret { get; set; } = "";

    public string Icon { get; set; } = "";

    public AuthenticationTypes AuthenticationType { get; set; }

    public ThirdPartyIdpSelectDto()
    {

    }

    public ThirdPartyIdpSelectDto(Guid id, string name, string displayName, string clientId, string clientSecret, string icon, AuthenticationTypes authenticationType)
    {
        Id = id;
        Name = name;
        DisplayName = displayName;
        ClientId = clientId;
        ClientSecret = clientSecret;
        Icon = icon;
        AuthenticationType = authenticationType;
    }
}

