﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Admin.Rcl.Pages.Subjects.ThirdPartyIdps;

public partial class UpdateThirdPartyIdpDialog
{
    [Parameter]
    public bool Visible { get; set; }

    [Parameter]
    public EventCallback<bool> VisibleChanged { get; set; }

    [Parameter]
    public EventCallback OnSubmitSuccess { get; set; }

    [Parameter]
    public Guid ThirdPartyIdpId { get; set; }

    private ThirdPartyIdpDetailDto ThirdPartyIdpDetail { get; set; } = new();

    private UpdateThirdPartyIdpDto ThirdPartyIdp { get; set; } = new();

    private ThirdPartyIdpService ThirdPartyIdpService => AuthCaller.ThirdPartyIdpService;

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
            await GetThirdPartyIdpDetailAsync();
        }
    }

    public async Task GetThirdPartyIdpDetailAsync()
    {
        ThirdPartyIdpDetail = await ThirdPartyIdpService.GetDetailAsync(ThirdPartyIdpId);
        ThirdPartyIdp = ThirdPartyIdpDetail;
    }

    public async Task UpdateThirdPartyIdpAsync(EditContext context)
    {
        var success = context.Validate();
        if (success)
        {
            Loading = true;
            await ThirdPartyIdpService.UpdateAsync(ThirdPartyIdp);
            OpenSuccessMessage(T("Update thirdPartyIdp success"));
            await OnSubmitSuccess.InvokeAsync();
            await UpdateVisible(false);
            Loading = false;
        }
    }
}
