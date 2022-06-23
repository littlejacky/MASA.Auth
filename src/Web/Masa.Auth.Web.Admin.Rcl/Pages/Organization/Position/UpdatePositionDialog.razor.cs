﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Admin.Rcl.Pages.Organization.Position;

public partial class UpdatePositionDialog
{
    [Parameter]
    public bool Visible { get; set; }

    [Parameter]
    public EventCallback<bool> VisibleChanged { get; set; }

    [Parameter]
    public EventCallback OnSubmitSuccess { get; set; }

    [Parameter]
    public Guid PositionId { get; set; }

    private PositionDetailDto PositionDetail { get; set; } = new();

    private UpdatePositionDto Position { get; set; } = new();

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

    protected override async Task OnParametersSetAsync()
    {
        if (Visible)
        {
            await GetPositionDetailAsync();
        }
    }

    public async Task GetPositionDetailAsync()
    {
        PositionDetail = await PositionService.GetDetailAsync(PositionId);
        Position = PositionDetail;
    }

    public async Task UpdatetPositionAsync(EditContext context)
    {
        var success = context.Validate();
        if (success)
        {
            Loading = true;
            await PositionService.UpdateAsync(Position);
            OpenSuccessMessage("Update position success");
            await OnSubmitSuccess.InvokeAsync();
            await UpdateVisible(false);
            Loading = false;
        }
    }
}