using Powertech.Platform.Iam.Domain.Model.Aggregates;
using Powertech.Platform.Iam.Domain.Model.Commands;
using Powertech.Platform.Shared.Application.Model;

namespace Powertech.Platform.Iam.Application.CommandServices;

public interface IIamCommandService
{
    Task<Result<User>> Handle(SignUpCommand command, CancellationToken cancellationToken);

    Task<Result<(User User, string Token)>> Handle(SignInCommand command, CancellationToken cancellationToken);

    Task<Result<Organization>> Handle(CreateOrganizationCommand command, CancellationToken cancellationToken);

    Task<Result<Organization>> Handle(UpdateOrganizationCommand command, CancellationToken cancellationToken);
}
