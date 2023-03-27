﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Application.Subjects
{
    public class TeamCacheCommandHandler
    {
        readonly IMultilevelCacheClient _multilevelCacheClient;
        readonly ITeamRepository _teamRepository;

        public TeamCacheCommandHandler(AuthClientMultilevelCacheProvider authClientMultilevelCacheProvider, ITeamRepository teamRepository)
        {
            _multilevelCacheClient = authClientMultilevelCacheProvider.GetMultilevelCacheClient();
            _teamRepository = teamRepository;
        }

        [EventHandler(99)]
        public async Task AddTeamAsync(AddTeamCommand command)
        {
            await SetTeamCacheAsync(command.Result);
        }

        [EventHandler(99)]
        public async Task UpdateTeamAsync(UpdateTeamCommand command)
        {
            await SetTeamCacheAsync(command.UpdateTeamDto.Id);
        }

        [EventHandler(99)]
        public async Task RemoveTeamAsync(RemoveTeamCommand command)
        {
            var teamCache = await _multilevelCacheClient.GetAsync<TeamDetailDto>(CacheKey.TeamKey(command.TeamId));
            await _multilevelCacheClient.RemoveAsync<TeamDetailDto>(CacheKey.TeamKey(command.TeamId));

            if (teamCache != null)
            {
                var userIdKeys = teamCache!.TeamAdmin.Staffs.Select(e => CacheKey.StaffTeamKey(e)).Union(teamCache!.TeamMember.Staffs.Select(e => CacheKey.StaffTeamKey(e)));
                var userTeamCacheDic = await _multilevelCacheClient.GetListAsync<KeyValuePair<string, List<TeamDetailDto>>>(userIdKeys);
                foreach (var item in userTeamCacheDic)
                {
                    var userTeamCache = item.Value.FirstOrDefault(e => e.Id == teamCache.Id);
                    if (userTeamCache != null)
                    {
                        item.Value.Remove(userTeamCache);
                    }
                }
                await _multilevelCacheClient.SetListAsync(userTeamCacheDic.ToDictionary(e => e.Key, e => e.Value)!);
            }
        }

        private async Task SetTeamCacheAsync(Guid teamId)
        {
            var team = await _teamRepository.GetByIdAsync(teamId);
            await SetTeamCacheAsync(team);
        }

        private async Task SetTeamCacheAsync(Team team)
        {
            var teamDto = new TeamDetailDto
            {
                Id = team.Id,
                TeamBasicInfo = new TeamBasicInfoDto
                {
                    Name = team.Name,
                    Description = team.Description,
                    Type = (int)team.TeamType,
                    Avatar = new AvatarValueDto
                    {
                        Url = team.Avatar.Url,
                        Name = team.Avatar.Name,
                        Color = team.Avatar.Color
                    }
                },
                TeamAdmin = new TeamPersonnelDto
                {
                    Staffs = team.TeamStaffs.Where(s => s.TeamMemberType == TeamMemberTypes.Admin).Select(s => s.StaffId).ToList(),
                    Roles = team.TeamRoles.Where(r => r.TeamMemberType == TeamMemberTypes.Admin).Select(r => r.RoleId).ToList(),
                    Permissions = team.TeamPermissions.Where(p => p.TeamMemberType == TeamMemberTypes.Admin).Select(tp => (SubjectPermissionRelationDto)tp).ToList()
                },
                TeamMember = new TeamPersonnelDto
                {
                    Staffs = team.TeamStaffs.Where(s => s.TeamMemberType == TeamMemberTypes.Member).Select(s => s.StaffId).ToList(),
                    Roles = team.TeamRoles.Where(r => r.TeamMemberType == TeamMemberTypes.Member).Select(r => r.RoleId).ToList(),
                    Permissions = team.TeamPermissions.Where(p => p.TeamMemberType == TeamMemberTypes.Member).Select(tp => (SubjectPermissionRelationDto)tp).ToList()
                }
            };

            await _multilevelCacheClient.SetAsync(CacheKey.TeamKey(team.Id), teamDto);

            if (team.TeamStaffs.Count > 0)
            {
                foreach (var item in team.TeamStaffs.Select(e => e.StaffId).Distinct())
                {
                    var cacheStaffTeams = await _multilevelCacheClient.GetAsync<List<CacheStaffTeam>>(CacheKey.StaffTeamKey(item));
                    if (cacheStaffTeams == null)
                    {
                        cacheStaffTeams = new List<CacheStaffTeam>();
                    }
                    if (!cacheStaffTeams.Any(e => e.Id == team.Id))
                    {
                        cacheStaffTeams.Add(new CacheStaffTeam(teamDto.Id, team.TeamStaffs.First(e => e.StaffId == item).TeamMemberType));
                    }
                    await _multilevelCacheClient.SetAsync(CacheKey.StaffTeamKey(item), cacheStaffTeams);
                }
            }
        }

        [EventHandler]
        public async Task SyncTeamRedisAsync(SyncTeamRedisCommand command)
        {
            var teams = await _teamRepository.GetAllAsync();
            foreach (var item in teams)
            {
                await SetTeamCacheAsync(item);
            }
        }
    }
}