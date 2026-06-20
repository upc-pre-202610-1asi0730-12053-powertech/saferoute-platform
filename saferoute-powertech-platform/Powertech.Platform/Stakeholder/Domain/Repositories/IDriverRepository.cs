using Powertech.Platform.Shared.Domain.Model.ValueObjects;
using Powertech.Platform.Shared.Domain.Repositories;
using Powertech.Platform.Stakeholder.Domain.Model.Entities;

namespace Powertech.Platform.Stakeholder.Domain.Repositories;

public interface IDriverRepository : IBaseRepository<Driver>
{
    Task<Driver?> FindByDriverIdAsync(DriverId driverId, CancellationToken cancellationToken);
}
