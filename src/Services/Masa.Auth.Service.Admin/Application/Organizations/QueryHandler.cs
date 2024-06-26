﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Application.Organizations;

public class QueryHandler
{
    readonly IDepartmentRepository _departmentRepository;
    readonly IPositionRepository _positionRepository;

    public QueryHandler(IDepartmentRepository departmentRepository, IPositionRepository positionRepository)
    {
        _departmentRepository = departmentRepository;
        _positionRepository = positionRepository;
    }

    [EventHandler]
    public async Task GetDepartmentDetailAsync(DepartmentDetailQuery departmentDetailQuery)
    {
        var department = await _departmentRepository.GetByIdAsync(departmentDetailQuery.DepartmentId);
        departmentDetailQuery.Result = new DepartmentDetailDto
        {
            Id = department.Id,
            Name = department.Name,
            Description = department.Description,
            Enabled = department.Enabled,
            ParentId = department.ParentId,
            StaffList = department.DepartmentStaffs
                .Select(ds => ds.Staff)
                .Select(staff => (StaffDto)staff).ToList()
        };
    }

    [EventHandler]
    public async Task GetDepartmentTreeAsync(DepartmentTreeQuery departmentTreeQuery)
    {
        departmentTreeQuery.Result = await GetDepartmentsAsync(departmentTreeQuery.ParentId);
    }

    private async Task<List<DepartmentDto>> GetDepartmentsAsync(Guid parentId)
    {
        var result = new List<DepartmentDto>();
        //todo change memory
        var departments = await _departmentRepository.GetListAsync(d => d.ParentId == parentId);
        foreach (var department in departments)
        {
            var item = new DepartmentDto
            {
                Id = department.Id,
                Name = department.Name,
                Children = await GetDepartmentsAsync(department.Id),
                IsRoot = department.Level == 1
            };
            result.Add(item);
        }
        return result;
    }

    [EventHandler]
    public void DepartmentCount(DepartmentCountQuery departmentCountQuery)
    {
        var quantity = _departmentRepository.LevelQuantity();
        departmentCountQuery.Result = new DepartmentChildrenCountDto
        {
            SecondLevel = quantity.TryGetValue(2, out var val2) ? val2 : 0,
            ThirdLevel = quantity.TryGetValue(3, out var val3) ? val3 : 0,
            FourthLevel = quantity.TryGetValue(4, out var val4) ? val4 : 0,
        };
    }

    [EventHandler]
    public async Task GetPositionsAsync(PositionsQuery query)
    {
        Expression<Func<Position, bool>> condition = position => true;
        condition = condition.And(!string.IsNullOrEmpty(query.Search), position => position.Name.Contains(query.Search));

        var positions = await _positionRepository.GetPaginatedListAsync(condition, new PaginatedOptions
        {
            Page = query.Page,
            PageSize = query.PageSize,
            Sorting = new Dictionary<string, bool>
            {
                [nameof(Position.ModificationTime)] = true,
                [nameof(Position.CreationTime)] = true,
            }
        });

        query.Result = new(positions.Total, positions.Result.Select(position => new PositionDto(position.Id, position.Name)).ToList());
    }

    [EventHandler]
    public async Task GetPositionDetailAsync(PositionDetailQuery query)
    {
        var position = await _positionRepository.FindAsync(query.PositionId);
        if (position is null) throw new UserFriendlyException(errorCode: UserFriendlyExceptionCodes.POSITION_NOT_EXIST);

        query.Result = new PositionDetailDto(position.Id, position.Name);
    }

    [EventHandler]
    public async Task GetDepartmentSelectAsync(DepartmentSelectQuery query)
    {
        var departments = await _departmentRepository.GetListAsync(department => department.Name.Contains(query.Name));
        query.Result = departments.Select(department => new DepartmentSelectDto(department.Id, department.Name)).ToList();
    }

    [EventHandler]
    public async Task GetPositionSelectAsync(PositionSelectQuery query)
    {
        var psoitions = await _positionRepository.GetListAsync(p => p.Name.Contains(query.Name));
        query.Result = psoitions.Select(p => new PositionSelectDto(p.Id, p.Name)).ToList();
    }
}

