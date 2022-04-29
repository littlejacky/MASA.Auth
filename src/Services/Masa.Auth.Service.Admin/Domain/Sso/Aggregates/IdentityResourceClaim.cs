﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Domain.Sso.Aggregates;

public class IdentityResourceClaim : Entity<int>
{
    public int UserClaimId { get; private set; }

    public UserClaim UserClaim { get; private set; } = null!;

    public int IdentityResourceId { get; private set; }

    public IdentityResource IdentityResource { get; private set; } = null!;

    public IdentityResourceClaim(int userClaimId)
    {
        UserClaimId = userClaimId;
    }
}

