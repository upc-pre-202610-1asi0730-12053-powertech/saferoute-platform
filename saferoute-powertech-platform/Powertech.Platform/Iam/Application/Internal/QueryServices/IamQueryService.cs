using Powertech.Platform.Iam.Application.QueryServices;
using Powertech.Platform.Iam.Domain.Model.Aggregates;
using Powertech.Platform.Iam.Domain.Model.Queries;
using Powertech.Platform.Iam.Domain.Repositories;
using Powertech.Platform.Shared.Domain.Model.ValueObjects;

namespace Powertech.Platform.Iam.Application.Internal.QueryServices;

public class IamQueryService(IUserRepository userRepository, IOrganizationRepository organizationRepository)
    : IIamQueryService
{
    public Task<User?> Handle(GetUserByIdQuery query, CancellationToken cancellationToken) =>
        userRepository.FindByUserIdAsync(new UserId(query.UserId), cancellationToken);

    public async Task<IEnumerable<User>> Handle(GetAllUsersQuery query, CancellationToken cancellationToken) =>
        await userRepository.ListAsync(cancellationToken);

    public Task<Organization?> Handle(GetOrganizationByIdQuery query, CancellationToken cancellationToken) =>
        organizationRepository.FindByOrganizationIdAsync(new OrganizationId(query.OrganizationId),
            cancellationToken);
}
