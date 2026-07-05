using Powertech.Platform.Iam.Application.Internal.OutboundServices;
using BCryptNet = BCrypt.Net.BCrypt;

namespace Powertech.Platform.Iam.Infrastructure.Hashing.BCrypt.Services;

public class HashingService : IHashingService
{
    public string HashPassword(string password) => BCryptNet.HashPassword(password);

    public bool VerifyPassword(string password, string passwordHash) => BCryptNet.Verify(password, passwordHash);
}
