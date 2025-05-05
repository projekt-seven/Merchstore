using MerchStore.Domain.Entities;
using MerchStore.Domain.ValueObjects;
using Xunit;

namespace MerchStore.Domain.UnitTests.Entities;

public class OrderTests
{
    [Fact]
    public void Constructor_ValidParameters_ShouldCreateOrder()
    {
        // Arrange
        var customer = CreateCustomer();

        // Act
        var order = new Order(customer);

        // Assert
        Assert.NotNull(order);
        Assert.Equal(customer, order.Customer);
        Assert.Empty(order.Items);
        Assert.Equal(Order.OrderStatus.Pending, order.Status);
    }

    [Fact]
    public void Constructor_NullCustomer_ShouldThrowException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new Order(null!));
    }

    [Fact]
    public void AddItem_ValidItem_ShouldAddToOrder()
    {
        // Arrange
        var order = CreateOrder();
        var productId = Guid.NewGuid();
        var item = new OrderItem(productId, 2, Money.FromSEK(100));

        // Act
        order.AddItem(item);

        // Assert
        Assert.Single(order.Items);
        Assert.Equal(Money.FromSEK(200), order.TotalPrice);
    }

    [Fact]
    public void AddItem_NullItem_ShouldThrowException()
    {
        // Arrange
        var order = CreateOrder();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => order.AddItem(null!));
    }

    [Fact]
    public void RemoveItem_ExistingItem_ShouldRemoveFromOrder()
    {
        // Arrange
        var order = CreateOrder();
        var productId = Guid.NewGuid();
        var item = new OrderItem(productId, 2, Money.FromSEK(100));
        order.AddItem(item);

        // Act
        order.RemoveItem(item);

        // Assert
        Assert.Empty(order.Items);
        Assert.Equal(Money.FromSEK(0), order.TotalPrice);
    }

    [Fact]
    public void RemoveItem_NonExistingItem_ShouldThrowException()
    {
        // Arrange
        var order = CreateOrder();
        var productId = Guid.NewGuid();
        var item = new OrderItem(productId, 2, Money.FromSEK(100));

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => order.RemoveItem(item));
    }

    [Fact]
    public void UpdateItemQuantity_ValidQuantity_ShouldUpdateItem()
    {
        // Arrange
        var order = CreateOrder();
        var productId = Guid.NewGuid();
        var item = new OrderItem(productId, 2, Money.FromSEK(100));
        order.AddItem(item);

        // Act
        order.UpdateItemQuantity(item, 5);

        // Assert
        Assert.Equal(5, order.Items.First().Quantity);
        Assert.Equal(Money.FromSEK(500), order.TotalPrice);
    }

    [Fact]
    public void UpdateItemQuantity_InvalidQuantity_ShouldThrowException()
    {
        // Arrange
        var order = CreateOrder();
        var productId = Guid.NewGuid();
        var item = new OrderItem(productId, 2, Money.FromSEK(100));
        order.AddItem(item);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => order.UpdateItemQuantity(item, 0));
    }

    [Fact]
    public void MarkAsCompleted_ShouldUpdateStatus()
    {
        // Arrange
        var order = CreateOrder();

        // Act
        order.MarkAsCompleted();

        // Assert
        Assert.Equal(Order.OrderStatus.Completed, order.Status);
    }

    [Fact]
    public void CancelOrder_ShouldUpdateStatus()
    {
        // Arrange
        var order = CreateOrder();

        // Act
        order.CancelOrder();

        // Assert
        Assert.Equal(Order.OrderStatus.Cancelled, order.Status);
    }

    private Order CreateOrder()
    {
        var customer = CreateCustomer();
        return new Order(customer);
    }

    private Customer CreateCustomer()
    {
        return new Customer(
            "John",
            "Doe",
            "john.doe@example.com",
            "1234567890",
            "123 Street",
            "City",
            "12345"
        );
    }
}