namespace MarketPlace.Shared.Domain.Interfaces;

/// <summary>
/// Unit of Work contract for coordinating transactional changes across aggregates and persistence stores.
/// </summary>
public interface IUnitOfWork
{
    /// <summary>
    /// Saves all changes made in this context to the underlying database.
    /// </summary>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
    /// <returns>The number of state entries written to the database.</returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
