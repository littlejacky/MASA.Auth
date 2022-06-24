﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Admin.Rcl.Pages.Organization.Position;

public partial class AddPositionDialog
{
    [Parameter]
    public bool Visible { get; set; }

    [Parameter]
    public EventCallback<bool> VisibleChanged { get; set; }

    [Parameter]
    public EventCallback OnSubmitSuccess { get; set; }

    private AddPositionDto Position { get; set; } = new();

    private PositionService PositionService => AuthCaller.PositionService;

    private MForm? Form { get; set; }

    private async Task UpdateVisible(bool visible)
    {
        if (VisibleChanged.HasDelegate)
        {
            await VisibleChanged.InvokeAsync(visible);
        }
        else
        {
            Visible = visible;
        }
        if (Form is not null)
        {
            await Form.ResetValidationAsync();
        }
    }

    protected override void OnParametersSet()
    {
        if (Visible)
        {
            Position = new();
        }
    }

    public async Task AddPositionAsync(EditContext context)
    {
        var success = context.Validate();
        if (success)
        {
            Loading = true;
            await PositionService.AddAsync(Position);
            OpenSuccessMessage("Add position success");
            await UpdateVisible(false);
            await OnSubmitSuccess.InvokeAsync();            
            Loading = false;
        }
    }
}
