using Powertech.Platform.Shared.Domain.Model.ValueObjects;
using Powertech.Platform.Stakeholder.Domain.Model.Commands;
using Powertech.Platform.Stakeholder.Domain.Model.ValueObjects;

namespace Powertech.Platform.Stakeholder.Domain.Model.Entities;

public class Driver
{
    protected Driver()
    {
        Id = new DriverId();
        OrganizationId = new OrganizationId();
        UserId = new UserId();
        FullName = new FullName(string.Empty, string.Empty, false);
        Email = new Email(string.Empty, false);
        PhoneNumber = new PhoneNumber(string.Empty, false);
        LicenseNumber = new LicenseNumber(string.Empty, false);
    }

    public Driver(CreateDriverCommand command)
    {
        Id = DriverId.New();
        OrganizationId = new OrganizationId(command.OrganizationId);
        UserId = new UserId(command.UserId);
        FullName = new FullName(command.FirstName, command.LastName);
        Email = new Email(command.Email);
        PhoneNumber = new PhoneNumber(command.PhoneNumber);
        LicenseNumber = new LicenseNumber(command.LicenseNumber);
        Available = true;
    }

    public DriverId Id { get; private set; }

    public OrganizationId OrganizationId { get; private set; }

    public UserId UserId { get; private set; }

    public FullName FullName { get; private set; }

    public Email Email { get; private set; }

    public PhoneNumber PhoneNumber { get; private set; }

    public LicenseNumber LicenseNumber { get; private set; }

    public bool Available { get; private set; }

    public bool IsAvailable() => Available;

    public string GetFullName() => FullName.ToString();

    public string GetLicenseNumber() => LicenseNumber.ToString();

    public void UpdatePhoneNumber(PhoneNumber phone) => PhoneNumber = phone;

    public void Update(FullName fullName, Email email, PhoneNumber phoneNumber, LicenseNumber licenseNumber,
        bool available)
    {
        FullName = fullName;
        Email = email;
        PhoneNumber = phoneNumber;
        LicenseNumber = licenseNumber;
        Available = available;
    }
}
