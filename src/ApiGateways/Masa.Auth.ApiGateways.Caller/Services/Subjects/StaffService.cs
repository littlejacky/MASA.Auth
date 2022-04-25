﻿namespace Masa.Auth.ApiGateways.Caller.Services.Subjects;

public class StaffService : ServiceBase
{
    protected override string BaseUrl { get; set; }

    internal StaffService(ICallerProvider callerProvider) : base(callerProvider)
    {
        BaseUrl = "api/staff/";
    }

    public async Task<PaginationDto<StaffDto>> GetListAsync(GetStaffsDto request)
    {
        return await SendAsync<GetStaffsDto, PaginationDto<StaffDto>>(nameof(GetListAsync), request);
    }

    public async Task<List<StaffSelectDto>> GetSelectAsync(string name)
    {
        return await SendAsync<object, List<StaffSelectDto>>(nameof(GetSelectAsync), new { name });
    }

    public async Task<StaffDetailDto> GetDetailAsync(Guid id)
    {
        return await SendAsync<object, StaffDetailDto>(nameof(GetDetailAsync), new { id });
    }

    public async Task AddAsync(AddStaffDto request)
    {
        await SendAsync(nameof(AddAsync), request);
    }

    public async Task UpdateAsync(UpdateStaffDto request)
    {
        await SendAsync(nameof(UpdateAsync), request);
    }

    public async Task RemoveAsync(Guid id)
    {
        await SendAsync(nameof(RemoveAsync), new RemoveStaffDto(id));
    }
}

