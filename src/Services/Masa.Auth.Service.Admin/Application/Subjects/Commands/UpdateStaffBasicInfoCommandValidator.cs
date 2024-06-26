﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Application.Subjects.Commands;

public class UpdateStaffBasicInfoCommandValidator : MasaAbstractValidator<UpdateStaffBasicInfoCommand>
{
    public UpdateStaffBasicInfoCommandValidator(PhoneNumberValidator phoneValidator)
    {
        RuleFor(command => command.Staff.DisplayName).Required().MaximumLength(50);
        RuleFor(command => command.Staff.PhoneNumber).Required().SetValidator(phoneValidator);
        WhenNotEmpty(command => command.Staff.Email, r => r.Email());
    }
}
