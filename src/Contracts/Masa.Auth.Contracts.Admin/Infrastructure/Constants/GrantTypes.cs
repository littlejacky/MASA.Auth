﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Infrastructure.Constants;

public static class GrantTypes
{
    public const string Implicit = "implicit";

    public const string Hybrid = "hybrid";

    public const string AuthorizationCode = "authorization_code";

    public const string ClientCredentials = "client_credentials";

    public const string ResourceOwnerPassword = "password";

    public const string DeviceFlow = "urn:ietf:params:oauth:grant-type:device_code";
}
