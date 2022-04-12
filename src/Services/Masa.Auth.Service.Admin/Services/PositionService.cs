﻿namespace Masa.Auth.Service.Admin.Services;

public class PositionService : RestServiceBase
{
    public PositionService(IServiceCollection services) : base(services, "api/position")
    {

    }

    private async Task<StaffDetailDto> GetDetailAsync(IEventBus eventBus, [FromQuery] Guid id)
    {
        var query = new StaffDetailQuery(id);
        await eventBus.PublishAsync(query);
        return query.Result;
    }

    private async Task<List<PositionSelectDto>> GetSelectAsync(IEventBus eventBus,string name)
    {
        var query = new PositionSelectQuery(name);
        await eventBus.PublishAsync(query);
        return query.Result;
    }

    private async Task AddAsync(IEventBus eventBus,
        [FromBody] AddPositionDto position)
    {
        await eventBus.PublishAsync(new AddPositionCommand(position));
    }

    private async Task UpdateAsync(IEventBus eventBus,
        [FromBody] UpdatePositionDto position)
    {
        await eventBus.PublishAsync(new UpdatePositionCommand(position));
    }
}