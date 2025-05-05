using MerchStore.Domain.ValueObjects;

namespace MerchStore.Domain.Entities;

public class OrderItem
{
    public Guid Id { get; private set; }
    public Guid OrderId { get; private set; }
    public Guid ProductId { get; private set; }
    public int Quantity { get; private set; }
    public Money UnitPrice { get; private set; }
    public Money TotalPrice => UnitPrice * Quantity;

    // Add this navigation property
    public Order Order { get; private set; } // Navigation property to Order

    private OrderItem() { }

    public OrderItem(Guid productId, int quantity, Money unitPrice)
    {
        if (productId == Guid.Empty)
            throw new ArgumentException("Product ID cannot be empty", nameof(productId));
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero.", nameof(quantity));
        if (unitPrice == null || unitPrice.Amount <= 0)
            throw new ArgumentException("Unit price must be greater than zero.", nameof(unitPrice));

        Id = Guid.NewGuid();
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

    public void UpdateUnitPrice(Money newUnitPrice)
    {
        if (newUnitPrice == null || newUnitPrice.Amount <= 0)
            throw new ArgumentException("Unit price must be greater than zero.", nameof(newUnitPrice));

        UnitPrice = newUnitPrice;
    }

    public override bool Equals(object? obj)
    {
        if (obj is OrderItem other)
        {
            return Id == other.Id; // Compare by unique identifier
        }
        return false;
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }
}