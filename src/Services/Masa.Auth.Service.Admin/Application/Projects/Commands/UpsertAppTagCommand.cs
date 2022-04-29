﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Application.Projects.Commands;

public record UpsertAppTagCommand(AppTagDetailDto AppTagDetail) : Command
{
}
