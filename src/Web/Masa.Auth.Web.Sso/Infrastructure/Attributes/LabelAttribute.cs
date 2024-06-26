﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Sso.Infrastructure.Attributes;

public class LabelAttribute : Attribute
{
    public string Key { get; set; } = string.Empty;

    public LabelAttribute(string key)
    {
        Key = key;
    }
}
