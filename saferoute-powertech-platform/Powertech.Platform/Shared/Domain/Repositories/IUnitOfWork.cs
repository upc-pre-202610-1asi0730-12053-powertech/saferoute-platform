namespace Powertech.Platform.Shared.Domain.Repositories;
/// VA EN DOMINIO PORQUE SE ESTA APLICANDO EL PATTERN SEPARATED INTERFACE
///Y LA IMPLEMENTACION DE ESTA EN LA CAPA DE INFRAESTRUCTURA

/// <summary>
///     Unit of work interface for all repositories
/// </summary>
public interface IUnitOfWork
{
    /// <summary>
    ///     Save changes to the repository
    /// </summary>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns></returns>
    Task CompleteAsync(CancellationToken cancellationToken = default);
}