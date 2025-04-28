using MerchStore.Domain.ValueObjects;

namespace MerchStore.Domain.Entities;

public class OrderItem
{
    public Guid ProductId { get; private set; }
    public int Quantity { get; private set; }
    public Money UnitPrice { get; private set; }
    public Money TotalPrice => UnitPrice * Quantity;

    public OrderItem(Guid productId, int quantity, Money unitPrice)
    {
        if (productId == Guid.Empty)
            throw new ArgumentException("Product ID cannot be empty", nameof(productId));
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero.", nameof(quantity));
        if (unitPrice == null || unitPrice.Amount <= 0)
            throw new ArgumentException("Unit price must be greater than zero.", nameof(unitPrice));

        ProductId = productId;
        Quantity = quantity;
        UnitPrice = unitPrice;
    }

    public void UpdateQuantity(int newQuantity)
    {
        if (newQuantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero.", nameof(newQuantity));

        Quantity = newQuantity;
    }
}