using Microsoft.EntityFrameworkCore;
using Powertech.Platform.Shared.Domain.Model.ValueObjects;
using Powertech.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using Powertech.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using Powertech.Platform.Stakeholder.Domain.Model.Entities;
using Powertech.Platform.Stakeholder.Domain.Repositories;

namespace Powertech.Platform.Stakeholder.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

public class DriverRepository(AppDbContext context) : BaseRepository<Driver>(context), IDriverRepository
{
    public Task<Driver?> FindByDriverIdAsync(DriverId driverId, CancellationToken cancellationToken) =>
        Context.Set<Driver>().FirstOrDefaultAsync(driver => driver.Id == driverId, cancellationToken);
}
