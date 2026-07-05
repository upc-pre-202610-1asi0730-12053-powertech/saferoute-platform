using Microsoft.EntityFrameworkCore;
using Powertech.Platform.Iam.Domain.Model.Aggregates;
using Powertech.Platform.Iam.Domain.Model.ValueObjects;
using Powertech.Platform.Iam.Domain.Repositories;
using Powertech.Platform.Shared.Domain.Model.ValueObjects;
using Powertech.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using Powertech.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

namespace Powertech.Platform.Iam.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

public class UserRepository(AppDbContext context) : BaseRepository<User>(context), IUserRepository
{
    public Task<User?> FindByUserIdAsync(UserId userId, CancellationToken cancellationToken) =>
        Context.Set<User>().FirstOrDefaultAsync(user => user.Id == userId, cancellationToken);

    public Task<User?> FindByEmailAsync(string email, CancellationToken cancellationToken)
    {
        var normalizedEmail = new Email(email, false);
        return Context.Set<User>().FirstOrDefaultAsync(user => user.Email == normalizedEmail, cancellationToken);
    }

    public Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken)
    {
        var normalizedEmail = new Email(email, false);
        return Context.Set<User>().AnyAsync(user => user.Email == normalizedEmail, cancellationToken);
    }
}
