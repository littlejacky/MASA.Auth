﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Infrastructure.Models;

public class SmsOptions : ConfigurationApiMasaConfigurationOptions
{
    [JsonIgnore]
    public override string AppId => "public-$Config";

    [JsonIgnore]
    public override string? ObjectName { get; } = "$public.Sms";

    public string ChannelCode { get; set; } = "";

    public string TemplateCode { get; set; } = "";
}