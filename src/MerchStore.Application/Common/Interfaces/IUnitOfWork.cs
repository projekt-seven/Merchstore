namespace MerchStore.Application.Common.Interfaces;

/// <summary>
/// Interface for the Unit of Work pattern.
/// This defines operations for transactional work that spans multiple repositories.
/// </summary>
public interface IUnitOfWork
{
    /// <summary>
    /// Saves all changes made in the context to the database
    /// </summary>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Begins a new transaction
    /// </summary>
    Task BeginTransactionAsync();

    /// <summary>
    /// Commits all changes made in the current transaction
    /// </summary>
    Task CommitTransactionAsync();

    /// <summary>
    /// Rolls back all changes made in the current transaction
    /// </summary>
    Task RollbackTransactionAsync();
}