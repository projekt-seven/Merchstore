using MerchStore.Domain.Interfaces;

namespace MerchStore.Application.Common.Interfaces;

public interface IRepositoryManager
{
    /// <summary>
    /// Gets the product repository.
    /// </summary>
    IProductRepository ProductRepository { get; }

    /// <summary>
    /// Gets the unit of work to commit transactions.
    /// </summary>
    IUnitOfWork UnitOfWork { get; }
}