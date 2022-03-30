﻿global using FluentValidation;
global using FluentValidation.AspNetCore;
global using Masa.Auth.Contracts.Admin.Infrastructure.Dtos;
global using Masa.Auth.Contracts.Admin.Infrastructure.Enums;
global using Masa.Auth.Contracts.Admin.Organizations;
global using Masa.Auth.Contracts.Admin.Permissions;
global using Masa.Auth.Contracts.Admin.Subjects;
global using Masa.Auth.Service.Admin.Application.Organizations.Commands;
global using Masa.Auth.Service.Admin.Application.Organizations.Queries;
global using Masa.Auth.Service.Admin.Application.Permissions.Commands;
global using Masa.Auth.Service.Admin.Application.Permissions.Queries;
global using Masa.Auth.Service.Admin.Application.Subjects.Commands;
global using Masa.Auth.Service.Admin.Application.Subjects.Queries;
global using Masa.Auth.Service.Admin.Domain.Organizations.Aggregates;
global using Masa.Auth.Service.Admin.Domain.Organizations.Repositories;
global using Masa.Auth.Service.Admin.Domain.Permissions.Aggregates;
global using Masa.Auth.Service.Admin.Domain.Permissions.Repositories;
global using Masa.Auth.Service.Admin.Domain.Sso.Aggregates;
global using Masa.Auth.Service.Admin.Domain.Sso.Aggregates.Abstract;
global using Masa.Auth.Service.Admin.Domain.Subjects.Aggregates;
global using Masa.Auth.Service.Admin.Domain.Subjects.Repositories;
global using Masa.Auth.Service.Admin.Domain.Subjects.Services;
global using Masa.Auth.Service.Admin.Infrastructure;
global using Masa.Auth.Service.Admin.Infrastructure.Extensions;
global using Masa.Auth.Service.Admin.Infrastructure.Middleware;
global using Masa.BuildingBlocks.Configuration;
global using Masa.BuildingBlocks.Data.UoW;
global using Masa.BuildingBlocks.Ddd.Domain.Entities;
global using Masa.BuildingBlocks.Ddd.Domain.Entities.Auditing;
global using Masa.BuildingBlocks.Ddd.Domain.Events;
global using Masa.BuildingBlocks.Ddd.Domain.Repositories;
global using Masa.BuildingBlocks.Ddd.Domain.Values;
global using Masa.BuildingBlocks.Dispatcher.Events;
global using Masa.Contrib.Configuration;
global using Masa.Contrib.Data.UoW.EF;
global using Masa.Contrib.Ddd.Domain;
global using Masa.Contrib.Ddd.Domain.Repository.EF;
global using Masa.Contrib.Dispatcher.Events;
global using Masa.Contrib.Dispatcher.IntegrationEvents.Dapr;
global using Masa.Contrib.Dispatcher.IntegrationEvents.EventLogs.EF;
global using Masa.Contrib.ReadWriteSpliting.Cqrs.Commands;
global using Masa.Contrib.ReadWriteSpliting.Cqrs.Queries;
global using Masa.Contrib.Service.MinimalAPIs;
global using Masa.Utils.Caching.Redis.DependencyInjection;
global using Masa.Utils.Caching.Redis.Models;
global using Masa.Utils.Data.EntityFrameworkCore;
global using Microsoft.AspNetCore.Authentication.JwtBearer;
global using Microsoft.AspNetCore.Mvc;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.EntityFrameworkCore.Infrastructure;
global using Microsoft.EntityFrameworkCore.Metadata.Builders;
global using Microsoft.OpenApi.Models;
global using System.Linq.Expressions;
global using System.Reflection;
