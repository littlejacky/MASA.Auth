﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Admin.Rcl.Pages.Sso.CustomLoginRegister;

public partial class CustomLoginRegister
{
    private string? _search;
    private int _page = 1, _pageSize = 20;

    public string Search
    {
        get { return _search ?? ""; }
        set
        {
            _search = value;
            _page = 1;
            GetCustomLoginsAsync().ContinueWith(_ => InvokeAsync(StateHasChanged));
        }
    }

    public int Page
    {
        get { return _page; }
        set
        {
            _page = value;
            GetCustomLoginsAsync().ContinueWith(_ => InvokeAsync(StateHasChanged));
        }
    }

    public int PageSize
    {
        get { return _pageSize; }
        set
        {
            _page = 1;
            _pageSize = value;
            GetCustomLoginsAsync().ContinueWith(_ => InvokeAsync(StateHasChanged));
        }
    }

    public long Total { get; set; }

    public List<CustomLoginDto> CustomLogins { get; set; } = new();

    public int CurrentCustomLoginId { get; set; }

    public bool AddCustomLoginRegisterDialogVisible { get; set; }

    public bool UpdateCustomLoginRegisterDialogVisible { get; set; }

    private CustomLoginService CustomLoginService => AuthCaller.CustomLoginService;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await GetCustomLoginsAsync();
            StateHasChanged();
        }
        await base.OnAfterRenderAsync(firstRender);
    }

    protected override void OnInitialized()
    {
        PageName = "CustomLoginBlock";

        base.OnInitialized();

        _headers = new List<DataTableHeader<CustomLoginDto>> {
            new() { Text = T(nameof(ClientDto.LogoUri)), Value = nameof(ClientDto.LogoUri), Sortable = false, Width="105px" },
            new() { Text = T("TitleName"), Value = nameof(CustomLoginDto.Name), Sortable = false},
            new() { Text = T("ClientName"), Value = nameof(ClientDto.ClientName), Sortable = false},
            new() { Text = T(nameof(CustomLoginDto.CreationTime)), Value = nameof(CustomLoginDto.CreationTime), Sortable = false , Width="250px" },
            new() { Text = T(nameof(CustomLoginDto.ModificationTime)), Value = nameof(CustomLoginDto.ModificationTime), Sortable = false, Width="250px"  },
            new() { Text = T(nameof(CustomLoginDto.Modifier)), Value = nameof(CustomLoginDto.Modifier), Sortable = false, Width="105px" },
            new() { Text = T("State"), Value = nameof(CustomLoginDto.Enabled), Sortable = false , Width="105px"},
            new() { Text = T("Action"), Value = "Action", Sortable = false, Align = DataTableHeaderAlign.Center, Width="105px" },
        };
    }

    private List<DataTableHeader<CustomLoginDto>> _headers = new();

    public async Task GetCustomLoginsAsync()
    {
        var reuquest = new GetCustomLoginsDto(Page, PageSize, Search);
        var response = await CustomLoginService.GetListAsync(reuquest);
        CustomLogins = response.Items;
        Total = response.Total;
    }

    public void OpenAddCustomLoginDialog()
    {
        AddCustomLoginRegisterDialogVisible = true;
    }

    public void OpenUpdateCustomLoginDialog(CustomLoginDto CustomLogin)
    {
        CurrentCustomLoginId = CustomLogin.Id;
        UpdateCustomLoginRegisterDialogVisible = true;
    }

    public async Task OpenRemoveCustomLoginDialog(CustomLoginDto CustomLogin)
    {
        var confirm = await OpenConfirmDialog(T("Delete Custom Login"), T("Are you sure delete custom login \"{0}\"?", CustomLogin.Name));
        if (confirm) await RemoveCustomLoginAsync(CustomLogin.Id);
    }

    public async Task RemoveCustomLoginAsync(int CustomLoginId)
    {
        await CustomLoginService.RemoveAsync(CustomLoginId);
        OpenSuccessMessage(T("Delete Custom Login data success"));
        await GetCustomLoginsAsync();
    }
}

