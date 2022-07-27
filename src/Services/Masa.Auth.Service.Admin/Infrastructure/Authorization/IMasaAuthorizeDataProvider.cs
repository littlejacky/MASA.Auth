﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Infrastructure.Authorization;

public interface IMasaAuthorizeDataProvider
{
    string GetAccount();

    List<string> GetAllowCodeList();
}
