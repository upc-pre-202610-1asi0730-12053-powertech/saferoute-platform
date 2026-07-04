using Powertech.Platform.Iam.Domain.Model.Aggregates;

namespace Powertech.Platform.Iam.Application.Internal.OutboundServices;

public interface ITokenService
{
    string GenerateToken(User user);
}
