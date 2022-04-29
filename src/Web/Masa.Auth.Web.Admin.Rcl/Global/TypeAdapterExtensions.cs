﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using StackApp = Masa.Stack.Components.Models.App;
using StackNav = Masa.Stack.Components.Models.Nav;

namespace Masa.Auth.Web.Admin.Rcl.Global;

public static class TypeAdapterExtensions
{
    public static IServiceCollection AddTypeAdapter(this IServiceCollection services)
    {
        TypeAdapterConfig.GlobalSettings.Default.PreserveReference(true);
        TypeAdapterConfig<AppDto, StackApp>.ForType()
            .Map(dest => dest.Code, src => src.Identity);
        TypeAdapterConfig<PermissionNavDto, StackNav>.ForType();

        return services;
    }
}
