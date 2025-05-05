using MerchStore.Domain.Common;
using MerchStore.Domain.ValueObjects;

namespace MerchStore.Domain.Entities;

public class Order : Entity<Guid>
{
    public enum OrderStatus
    {
        Pending,
        Completed,
        Cancelled
    }

    public Customer Customer { get; private set; }
    private readonly List<OrderItem> _items = new List<OrderItem>();
    public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();
    public Money TotalPrice { get; private set; } = Money.FromSEK(0);
    public DateTime OrderDate { get; private set; } = DateTime.UtcNow;
    public OrderStatus Status { get; private set; } = OrderStatus.Pending;

    private Order() {} // Required for EF Core

    public Order(Customer customer) : base(Guid.NewGuid())
    {
        ArgumentNullException.ThrowIfNull(customer, nameof(customer));
        Customer = customer;
    }

    public void AddItem(OrderItem item)
    {
        ArgumentNullException.ThrowIfNull(item, nameof(item));

        _items.Add(item);
        RecalculateTotalPrice();
    }

    public void RemoveItem(OrderItem item)
    {
        if (!_items.Remove(item))
            throw new InvalidOperationException("Item not found in the order.");

        RecalculateTotalPrice();
    }

    public void UpdateItemQuantity(OrderItem item, int newQuantity)
    {
        if (newQuantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero.", nameof(newQuantity));

        var existingItem = _items.FirstOrDefault(i => i.Equals(item));
        if (existingItem == null)
            throw new InvalidOperationException("Item not found in the order.");

        existingItem.UpdateQuantity(newQuantity);
        RecalculateTotalPrice();
    }

    private void RecalculateTotalPrice()
    {
        TotalPrice = _items.Aggregate(Money.FromSEK(0), (total, item) => total + item.TotalPrice);
    }

    public void MarkAsCompleted()
    {
        Status = OrderStatus.Completed;
    }

    public void CancelOrder()
    {
        Status = OrderStatus.Cancelled;
    }
}