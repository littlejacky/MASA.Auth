﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

global using Dapr.Client;
global using FluentValidation;
global using FluentValidation.AspNetCore;
global using HealthChecks.UI.Client;
global using Magicodes.ExporterAndImporter.Csv;
global using Mapster;
global using Masa.Auth.Contracts.Admin.Infrastructure.Constants;
global using Masa.Auth.Contracts.Admin.Infrastructure.Dtos;
global using Masa.Auth.Contracts.Admin.Infrastructure.Enums;
global using Masa.Auth.Contracts.Admin.Infrastructure.Utils;
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
global using Masa.Auth.Service.Admin.Domain.Organizations.Aggregates;
global using Masa.Auth.Service.Admin.Domain.Organizations.Repositories;
global using Masa.Auth.Service.Admin.Domain.Permissions.Aggregates;
global using Masa.Auth.Service.Admin.Domain.Permissions.Repositories;
global using Masa.Auth.Service.Admin.Domain.Projects.Aggregates;
global using Masa.Auth.Service.Admin.Domain.Projects.Repositories;
global using Masa.Auth.Service.Admin.Domain.Sso.Aggregates;
global using Masa.Auth.Service.Admin.Domain.Sso.Repositories;
global using Masa.Auth.Service.Admin.Domain.Subjects.Aggregates;
global using Masa.Auth.Service.Admin.Domain.Subjects.Events;
global using Masa.Auth.Service.Admin.Domain.Subjects.Repositories;
global using Masa.Auth.Service.Admin.Domain.Subjects.Services;
global using Masa.Auth.Service.Admin.Infrastructure;
global using Masa.Auth.Service.Admin.Infrastructure.CacheModels;
global using Masa.Auth.Service.Admin.Infrastructure.Constants;
global using Masa.Auth.Service.Admin.Infrastructure.Extensions;
global using Masa.Auth.Service.Admin.Infrastructure.Middleware;
global using Masa.Auth.Service.Admin.Infrastructure.Models;
global using Masa.BuildingBlocks.Authentication.Oidc.Cache.Caches;
global using Masa.BuildingBlocks.Authentication.Oidc.Domain.Entities;
global using Masa.BuildingBlocks.Authentication.Oidc.Domain.Enums;
global using Masa.BuildingBlocks.Authentication.Oidc.Domain.Repositories;
global using Masa.BuildingBlocks.Authentication.Oidc.Models.Constans;
global using Masa.BuildingBlocks.BasicAbility.Auth.Contracts.Enum;
global using Masa.BuildingBlocks.BasicAbility.Auth.Contracts.Model;
global using Masa.BuildingBlocks.BasicAbility.Pm;
global using Masa.BuildingBlocks.Data.UoW;
global using Masa.BuildingBlocks.Ddd.Domain.Entities;
global using Masa.BuildingBlocks.Ddd.Domain.Entities.Full;
global using Masa.BuildingBlocks.Ddd.Domain.Events;
global using Masa.BuildingBlocks.Ddd.Domain.Repositories;
global using Masa.BuildingBlocks.Ddd.Domain.Values;
global using Masa.BuildingBlocks.Dispatcher.Events;
global using Masa.BuildingBlocks.Identity.IdentityModel;
global using Masa.BuildingBlocks.ReadWriteSpliting.Cqrs.Commands;
global using Masa.BuildingBlocks.ReadWriteSpliting.Cqrs.Queries;
global using Masa.BuildingBlocks.SearchEngine.AutoComplete;
global using Masa.BuildingBlocks.Storage.ObjectStorage;
global using Masa.Contrib.Authentication.Oidc.Cache;
global using Masa.Contrib.Authentication.Oidc.EntityFrameworkCore;
global using Masa.Contrib.Authentication.Oidc.EntityFrameworkCore.DbContexts;
global using Masa.Contrib.BasicAbility.Tsc;
global using Masa.Contrib.Data.Contracts.EF;
global using Masa.Contrib.Data.EntityFrameworkCore;
global using Masa.Contrib.Data.EntityFrameworkCore.SqlServer;
global using Masa.Contrib.Ddd.Domain;
global using Masa.Contrib.Ddd.Domain.Repository.EF;
global using Masa.Contrib.Dispatcher.Events;
global using Masa.Contrib.Dispatcher.IntegrationEvents;
global using Masa.Contrib.Dispatcher.IntegrationEvents.Dapr;
global using Masa.Contrib.Dispatcher.IntegrationEvents.EventLogs.EF;
global using Masa.Contrib.Isolation.MultiEnvironment;
global using Masa.Contrib.Isolation.UoW.EF;
global using Masa.Contrib.Service.MinimalAPIs;
global using Masa.Contrib.Storage.ObjectStorage.Aliyun.Options;
global using Masa.Utils.Caching.DistributedMemory.DependencyInjection;
global using Masa.Utils.Caching.DistributedMemory.Interfaces;
global using Masa.Utils.Caching.Redis.Models;
global using Masa.Utils.Ldap.Novell;
global using Masa.Utils.Security.Cryptography;
global using Microsoft.AspNetCore.Authentication.JwtBearer;
global using Microsoft.AspNetCore.Diagnostics.HealthChecks;
global using Microsoft.AspNetCore.Mvc;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.EntityFrameworkCore.Infrastructure;
global using Microsoft.EntityFrameworkCore.Metadata.Builders;
global using Microsoft.Extensions.DependencyInjection.Extensions;
global using Microsoft.Extensions.Diagnostics.HealthChecks;
global using Microsoft.OpenApi.Models;
global using OpenTelemetry.Logs;
global using OpenTelemetry.Metrics;
global using OpenTelemetry.Resources;
global using OpenTelemetry.Trace;
global using SixLabors.ImageSharp;
global using System.Diagnostics.CodeAnalysis;
global using System.Linq.Expressions;
global using System.Reflection;
global using System.Text.RegularExpressions;
global using Client = Masa.BuildingBlocks.Authentication.Oidc.Domain.Entities.Client;
global using Event = Masa.BuildingBlocks.Dispatcher.Events.Event;
