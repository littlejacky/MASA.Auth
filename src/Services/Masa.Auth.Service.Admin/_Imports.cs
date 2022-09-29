﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

global using FluentValidation;
global using FluentValidation.AspNetCore;
global using HealthChecks.UI.Client;
global using Magicodes.ExporterAndImporter.Core.Extension;
global using Magicodes.ExporterAndImporter.Csv;
global using Mapster;
global using Masa.Auth.Contracts.Admin.Infrastructure.Constants;
global using Masa.Auth.Contracts.Admin.Infrastructure.Dtos;
global using Masa.Auth.Contracts.Admin.Infrastructure.Enums;
global using Masa.Auth.Contracts.Admin.Infrastructure.Extensions;
global using Masa.Auth.Contracts.Admin.Infrastructure.Models;
global using Masa.Auth.Contracts.Admin.Infrastructure.Utils;
global using Masa.Auth.Contracts.Admin.Logs;
global using Masa.Auth.Contracts.Admin.Organizations;
global using Masa.Auth.Contracts.Admin.Organizations.Validator;
global using Masa.Auth.Contracts.Admin.Oss;
global using Masa.Auth.Contracts.Admin.Permissions;
global using Masa.Auth.Contracts.Admin.Permissions.Validator;
global using Masa.Auth.Contracts.Admin.Projects;
global using Masa.Auth.Contracts.Admin.Sso;
global using Masa.Auth.Contracts.Admin.Sso.Validator;
global using Masa.Auth.Contracts.Admin.Subjects;
global using Masa.Auth.Contracts.Admin.Subjects.Validator;
global using Masa.Auth.Security.OAuth.Providers;
global using Masa.Auth.Service.Admin.Application.Logs.Commands;
global using Masa.Auth.Service.Admin.Application.Logs.Queries;
global using Masa.Auth.Service.Admin.Application.Messages.Commands;
global using Masa.Auth.Service.Admin.Application.Organizations.Commands;
global using Masa.Auth.Service.Admin.Application.Organizations.Queries;
global using Masa.Auth.Service.Admin.Application.Permissions.Commands;
global using Masa.Auth.Service.Admin.Application.Permissions.Queries;
global using Masa.Auth.Service.Admin.Application.Projects.Commands;
global using Masa.Auth.Service.Admin.Application.Projects.Queries;
global using Masa.Auth.Service.Admin.Application.Sso.Commands;
global using Masa.Auth.Service.Admin.Application.Sso.Queries;
global using Masa.Auth.Service.Admin.Application.Subjects.Commands;
global using Masa.Auth.Service.Admin.Application.Subjects.Queries;
global using Masa.Auth.Service.Admin.Domain.Logs.Aggregates;
global using Masa.Auth.Service.Admin.Domain.Logs.Repositories;
global using Masa.Auth.Service.Admin.Domain.Organizations.Aggregates;
global using Masa.Auth.Service.Admin.Domain.Organizations.Repositories;
global using Masa.Auth.Service.Admin.Domain.Permissions.Aggregates;
global using Masa.Auth.Service.Admin.Domain.Permissions.Repositories;
global using Masa.Auth.Service.Admin.Domain.Permissions.Services;
global using Masa.Auth.Service.Admin.Domain.Projects.Aggregates;
global using Masa.Auth.Service.Admin.Domain.Projects.Repositories;
global using Masa.Auth.Service.Admin.Domain.Sso.Aggregates;
global using Masa.Auth.Service.Admin.Domain.Sso.Repositories;
global using Masa.Auth.Service.Admin.Domain.Subjects.Aggregates;
global using Masa.Auth.Service.Admin.Domain.Subjects.Events;
global using Masa.Auth.Service.Admin.Domain.Subjects.Repositories;
global using Masa.Auth.Service.Admin.Domain.Subjects.Services;
global using Masa.Auth.Service.Admin.Infrastructure;
global using Masa.Auth.Service.Admin.Infrastructure.Attributes;
global using Masa.Auth.Service.Admin.Infrastructure.Authorization;
global using Masa.Auth.Service.Admin.Infrastructure.CacheModels;
global using Masa.Auth.Service.Admin.Infrastructure.Constants;
global using Masa.Auth.Service.Admin.Infrastructure.Email;
global using Masa.Auth.Service.Admin.Infrastructure.Extensions;
global using Masa.Auth.Service.Admin.Infrastructure.Middleware;
global using Masa.Auth.Service.Admin.Infrastructure.Models;
global using Masa.Auth.Service.Admin.Infrastructure.Sms;
global using Masa.BuildingBlocks.Authentication.Identity;
global using Masa.BuildingBlocks.Authentication.OpenIdConnect.Cache.Caches;
global using Masa.BuildingBlocks.Authentication.OpenIdConnect.Domain.Entities;
global using Masa.BuildingBlocks.Authentication.OpenIdConnect.Domain.Enums;
global using Masa.BuildingBlocks.Authentication.OpenIdConnect.Domain.Repositories;
global using Masa.BuildingBlocks.Authentication.OpenIdConnect.Models.Constans;
global using Masa.BuildingBlocks.Caching;
global using Masa.BuildingBlocks.Configuration;
global using Masa.BuildingBlocks.Data.UoW;
global using Masa.BuildingBlocks.Ddd.Domain.Entities;
global using Masa.BuildingBlocks.Ddd.Domain.Entities.Full;
global using Masa.BuildingBlocks.Ddd.Domain.Events;
global using Masa.BuildingBlocks.Ddd.Domain.Repositories;
global using Masa.BuildingBlocks.Ddd.Domain.Values;
global using Masa.BuildingBlocks.Dispatcher.Events;
global using Masa.BuildingBlocks.Isolation.Environment;
global using Masa.BuildingBlocks.ReadWriteSplitting.Cqrs.Commands;
global using Masa.BuildingBlocks.ReadWriteSplitting.Cqrs.Queries;
global using Masa.BuildingBlocks.SearchEngine.AutoComplete;
global using Masa.BuildingBlocks.StackSdks.Auth.Contracts.Consts;
global using Masa.BuildingBlocks.StackSdks.Auth.Contracts.Enum;
global using Masa.BuildingBlocks.StackSdks.Auth.Contracts.Model;
global using Masa.BuildingBlocks.StackSdks.Dcc;
global using Masa.BuildingBlocks.StackSdks.Mc;
global using Masa.BuildingBlocks.StackSdks.Mc.Enum;
global using Masa.BuildingBlocks.StackSdks.Mc.Model;
global using Masa.BuildingBlocks.StackSdks.Pm;
global using Masa.BuildingBlocks.StackSdks.Pm.Enum;
global using Masa.BuildingBlocks.StackSdks.Scheduler;
global using Masa.BuildingBlocks.StackSdks.Scheduler.Enum;
global using Masa.BuildingBlocks.StackSdks.Scheduler.Model;
global using Masa.BuildingBlocks.StackSdks.Scheduler.Request;
global using Masa.BuildingBlocks.Storage.ObjectStorage;
global using Masa.Contrib.Authentication.OpenIdConnect.EFCore.Caches;
global using Masa.Contrib.Authentication.OpenIdConnect.EFCore.DbContexts;
global using Masa.Contrib.Authentication.OpenIdConnect.EFCore.Repositories;
global using Masa.Contrib.Caching.Distributed.StackExchangeRedis;
global using Masa.Contrib.Ddd.Domain;
global using Masa.Contrib.Ddd.Domain.Repository.EFCore;
global using Masa.Contrib.Dispatcher.Events;
global using Masa.Contrib.Dispatcher.IntegrationEvents;
global using Masa.Contrib.Dispatcher.IntegrationEvents.Dapr;
global using Masa.Contrib.Dispatcher.IntegrationEvents.EventLogs.EFCore;
global using Masa.Contrib.Isolation.MultiEnvironment;
global using Masa.Contrib.Isolation.UoW.EFCore;
global using Masa.Contrib.Service.MinimalAPIs;
global using Masa.Contrib.StackSdks.Tsc;
global using Masa.Contrib.Storage.ObjectStorage.Aliyun.Options;
global using Masa.Utils.Ldap.Novell;
global using Masa.Utils.Security.Cryptography;
global using Microsoft.AspNetCore.Authentication.JwtBearer;
global using Microsoft.AspNetCore.Authorization;
global using Microsoft.AspNetCore.Authorization.Policy;
global using Microsoft.AspNetCore.Diagnostics.HealthChecks;
global using Microsoft.AspNetCore.Mvc;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.EntityFrameworkCore.Design;
global using Microsoft.EntityFrameworkCore.Infrastructure;
global using Microsoft.EntityFrameworkCore.Metadata.Builders;
global using Microsoft.Extensions.DependencyInjection.Extensions;
global using Microsoft.Extensions.Diagnostics.HealthChecks;
global using Microsoft.Extensions.Options;
global using Microsoft.OpenApi.Models;
global using Novell.Directory.Ldap;
global using OpenTelemetry.Logs;
global using OpenTelemetry.Metrics;
global using OpenTelemetry.Resources;
global using OpenTelemetry.Trace;
global using SixLabors.ImageSharp;
global using System.Collections.Concurrent;
global using System.Diagnostics.CodeAnalysis;
global using System.Linq.Expressions;
global using System.Reflection;
global using System.Runtime.CompilerServices;
global using System.Text.Json;
global using System.Text.RegularExpressions;
global using IdentityProvider = Masa.Auth.Service.Admin.Domain.Subjects.Aggregates.IdentityProvider;
global using Client = Masa.BuildingBlocks.Authentication.OpenIdConnect.Domain.Entities.Client;
global using Event = Masa.BuildingBlocks.Dispatcher.Events.Event;
