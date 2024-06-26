﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Subjects;

public class AddThirdPartyUserDto
{
    public Guid ThirdPartyIdpId { get; set; }

    public bool Enabled { get; set; } = true;

    public string ThridPartyIdentity { get; set; } = "";

    public string ExtendedData { get; private set; } = "";

    public AddUserDto User { get; set; } = new();

    public Dictionary<string, string> ClaimData { get; set; } = new();

    public bool IsLdap { get; set; }

    public AddThirdPartyUserDto()
    {

    }

    public AddThirdPartyUserDto(Guid thirdPartyIdpId, bool enabled, string thridPartyIdentity, string extendedData, AddUserDto user, Dictionary<string, string> claimData)
    {
        ThirdPartyIdpId = thirdPartyIdpId;
        Enabled = enabled;
        ThridPartyIdentity = thridPartyIdentity;
        ExtendedData = extendedData;
        User = user;
        ClaimData = claimData;
    }
}
