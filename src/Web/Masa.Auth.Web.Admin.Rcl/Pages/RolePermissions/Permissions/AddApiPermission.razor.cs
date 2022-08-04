﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Admin.Rcl.Pages.RolePermissions.Permissions;

public partial class AddApiPermission
{
    [EditorRequired]
    [Parameter]
    public List<AppDto> AppItems { get; set; } = new();

    [Parameter]
    public EventCallback<ApiPermissionDetailDto> OnSubmit { get; set; }

    MForm _form = default!;
    ApiPermissionDetailDto _apiPermissionDetailDto = new();
    bool _visible { get; set; }
    List<SelectItemDto<PermissionTypes>> _apiPermissionTypes = new();

    PermissionService PermissionService => AuthCaller.PermissionService;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            var permissionTypes = await PermissionService.GetTypesAsync();
            _apiPermissionTypes = permissionTypes.Where(a => a.Value == PermissionTypes.Api).ToList();
        }
        await base.OnAfterRenderAsync(firstRender);
    }

    private async Task OnSubmitHandler()
    {
        if (await _form.ValidateAsync())
        {
            if (OnSubmit.HasDelegate)
            {
                await OnSubmit.InvokeAsync(_apiPermissionDetailDto);
            }
            _visible = false;
        }
    }

    public void Show()
    {
        _apiPermissionDetailDto = new();
        _visible = true;
    }
}
