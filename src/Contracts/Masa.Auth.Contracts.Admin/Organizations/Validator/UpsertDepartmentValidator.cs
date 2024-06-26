﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Organizations.Validator;

public class UpsertDepartmentValidator : AbstractValidator<UpsertDepartmentDto>
{
    public UpsertDepartmentValidator()
    {
        RuleFor(d => d.Name).Required();
    }
}
