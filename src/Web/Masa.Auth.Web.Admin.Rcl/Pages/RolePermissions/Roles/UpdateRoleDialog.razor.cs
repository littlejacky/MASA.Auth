﻿namespace Masa.Auth.Web.Admin.Rcl.Pages.RolePermissions.Roles;

public partial class UpdateRoleDialog
{
    [Parameter]
    public bool Visible { get; set; }

    [Parameter]
    public EventCallback<bool> VisibleChanged { get; set; }

    [Parameter]
    public EventCallback OnSubmitSuccess { get; set; }

    [Parameter]
    public Guid RoleId { get; set; }

    private UpdateRoleDto Role { get; set; } = new();

    private RoleDetailDto RoleDetail { get; set; } = new();

    private RoleService RoleService => AuthCaller.RoleService;

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
            await GetRoleDetailAsync();
        }
    }

    public async Task GetRoleDetailAsync()
    {
        RoleDetail = await RoleService.GetDetailAsync(RoleId);
        Role = RoleDetail;
    }

    public async Task UpdateRoleAsync(EditContext context)
    {
        var success = context.Validate();
        if(success)
        {
            Loading = true;
            await RoleService.UpdateAsync(Role);
            OpenSuccessMessage(T("Update role data success"));
            await UpdateVisible(false);
            await OnSubmitSuccess.InvokeAsync();           
            Loading = false;
        }
    }
}
