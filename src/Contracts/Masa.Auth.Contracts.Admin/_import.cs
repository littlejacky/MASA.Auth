﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

global using FluentValidation;
global using Magicodes.ExporterAndImporter.Core;
global using Masa.Auth.Contracts.Admin.Infrastructure.Constants;
global using Masa.Auth.Contracts.Admin.Infrastructure.Converters;
global using Masa.Auth.Contracts.Admin.Infrastructure.Dtos;
global using Masa.Auth.Contracts.Admin.Infrastructure.Enums;
global using Masa.Auth.Contracts.Admin.Infrastructure.Models;
global using Masa.Auth.Contracts.Admin.Infrastructure.Utils;
global using Masa.Auth.Contracts.Admin.Permissions;
global using Masa.Auth.Contracts.Admin.Subjects;
global using Masa.BuildingBlocks.Authentication.OpenIdConnect.Domain.Enums;
global using Masa.BuildingBlocks.Authentication.OpenIdConnect.Models.Enums;
global using Masa.BuildingBlocks.SearchEngine.AutoComplete;
global using Masa.BuildingBlocks.StackSdks.Auth.Contracts.Enum;
global using Masa.Contrib.Configuration.ConfigurationApi.Dcc.Options;
global using Microsoft.AspNetCore.Http;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Options;
global using Microsoft.Extensions.Primitives;
global using SixLabors.Fonts;
global using SixLabors.ImageSharp;
global using SixLabors.ImageSharp.Drawing.Processing;
global using SixLabors.ImageSharp.PixelFormats;
global using SixLabors.ImageSharp.Processing;
global using System.Collections.Concurrent;
global using System.ComponentModel;
global using System.Diagnostics.CodeAnalysis;
global using System.Numerics;
global using System.Reflection;
global using System.Runtime.CompilerServices;
global using System.Text;
global using System.Text.Json;
global using System.Text.Json.Serialization;
global using Util.Reflection.Expressions;
global using Util.Reflection.Expressions.Abstractions;
