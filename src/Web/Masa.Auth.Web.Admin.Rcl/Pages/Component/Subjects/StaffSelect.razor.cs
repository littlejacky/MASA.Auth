﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Admin.Rcl.Pages.Component.Subjects;

public partial class StaffSelect
{
    [Parameter]
    public List<Guid> Value { get; set; } = new();

    [Parameter]
    public EventCallback<List<Guid>> ValueChanged { get; set; }

    [Parameter]
    public string Label { get; set; } = "";

    [Parameter]
    public bool Readonly { get; set; }

    [Parameter]
    public string Class { get; set; } = "";

    [Parameter]
    public RoleLimitModel RoleLimit { get; set; } = new("", int.MaxValue);

    protected List<StaffSelectDto> Staffs { get; set; } = new();

    protected StaffService StaffService => AuthCaller.StaffService;

    protected override async Task OnInitializedAsync()
    {
        Label = T("Staff");
        Staffs = await StaffService.GetSelectAsync();
    }

    protected async Task RemoveStaff(StaffSelectDto staff)
    {
        if (Readonly is false)
        {
            var value = new List<Guid>();
            value.AddRange(Value);
            value.Remove(staff.Id);
            await UpdateValueAsync(value);
        }
    }

    public async Task UpdateValueAsync(List<Guid> value)
    {
        if (value.Count > RoleLimit.Limit)
        {
            value.Remove(value.Last());
            OpenErrorMessage(string.Format(T("Due to the role [{0}] limit constraint, a maximum of {1} members can be selected"), RoleLimit.Role, RoleLimit.Limit));
        }
        else
        {
            if (ValueChanged.HasDelegate) await ValueChanged.InvokeAsync(value);
            else Value = value;
        }
    }
}
