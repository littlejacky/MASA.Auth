﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Admin.Rcl.Pages.Component;

public class DefaultChangeImage : UploadAvatar
{
    private GenderTypes _gender { get; set; }

    [Inject]
    public AuthCaller AuthCaller { get; set; } = default!;

    [Parameter]
    public GenderTypes Gender
    {
        get => _gender;
        set
        {
            if (value != _gender)
            {
                _gender = value;
                if (DefaultImages.Any(image => image.Url == Value))
                {
                    ChangeAvayarAsync().ContinueWith(_ => InvokeAsync(StateHasChanged));
                }
            }
        }
    }

    private List<GetDefaultImagesDto> DefaultImages { get; set; } = new();

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        DefaultImages = await AuthCaller.OssService.GetDefaultImagesAsync();
    }

    protected override async Task OnParametersSetAsync()
    {
        await base.OnParametersSetAsync();
        if (string.IsNullOrEmpty(Value)) await ChangeAvayarAsync();
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        base.BuildRenderTree(builder);
        builder.OpenRegion(18);
        builder.OpenElement(0, "div");
        builder.AddAttribute(1, "class", "mt-3 hover-pointer body primary--text");
        builder.AddAttribute(2, "onclick", EventCallback.Factory.Create(this, ChangeAvayarAsync));

        builder.OpenComponent<SIcon>(3);
        builder.AddAttribute(4, "Size", (StringNumber)20);
        builder.AddAttribute(5, "Class", "primary--text");
        builder.AddAttribute(6, "ChildContent", (RenderFragment)delegate (RenderTreeBuilder builder2) {
            builder2.AddContent(7, IconConstants.Refresh);
        });
        builder.CloseComponent();

        builder.OpenElement(8, "span");
        builder.AddAttribute(9, "class", "body ml-2");
        builder.AddContent(10, I18n.T("Another", true));
        builder.CloseElement();

        builder.CloseElement();
        builder.CloseRegion();
    }

    private async Task ChangeAvayarAsync()
    {
        Random random = new Random();
        var images = DefaultImages.Where(image => image.Gender == Gender).ToList();
        if (images.Count > 0)
        {
            var avatar = images[random.Next(0, images.Count)].Url;
            if (avatar == Value && images.Count > 1) await ChangeAvayarAsync();
            else
            {
                await SetValueAsync(avatar);
            }
        }
    }
}
