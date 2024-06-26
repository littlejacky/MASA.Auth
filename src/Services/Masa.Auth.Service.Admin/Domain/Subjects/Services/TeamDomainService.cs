﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Domain.Subjects.Services;

public class TeamDomainService : DomainService
{
    public TeamDomainService(IDomainEventBus eventBus) : base(eventBus)
    {
    }

    public async Task SetTeamAdminAsync(Team team, List<Guid> staffIds, List<Guid> roleIds, List<SubjectPermissionRelationDto> permissions)
    {
        var _event = new SetTeamPersonnelInfoDomainEvent(team, TeamMemberTypes.Admin, staffIds, roleIds, permissions);
        await EventBus.PublishAsync(_event);
    }

    public async Task SetTeamMemberAsync(Team team, List<Guid> staffIds, List<Guid> roleIds, List<SubjectPermissionRelationDto> permissions)
    {
        var _event = new SetTeamPersonnelInfoDomainEvent(team, TeamMemberTypes.Member, staffIds, roleIds, permissions);
        await EventBus.PublishAsync(_event);
    }
}
