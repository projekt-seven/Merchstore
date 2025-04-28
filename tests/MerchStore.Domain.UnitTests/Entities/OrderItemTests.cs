using MerchStore.Domain.Entities;
using MerchStore.Domain.ValueObjects;
using Xunit;

namespace MerchStore.Domain.UnitTests.Entities
{
    public class OrderItemTests
    {
        [Fact]
        public void Equals_SameValues_ReturnsTrue()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var unitPrice = new Money(10.0m, "SEK");
            var orderItem1 = new OrderItem(productId, 5, unitPrice);
            var orderItem2 = new OrderItem(productId, 5, unitPrice);

            // Act
            var result = orderItem1.Equals(orderItem2);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void Equals_DifferentProductId_ReturnsFalse()
        {
            // Arrange
            var unitPrice = new Money(10.0m, "SEK");
            var orderItem1 = new OrderItem(Guid.NewGuid(), 5, unitPrice);
            var orderItem2 = new OrderItem(Guid.NewGuid(), 5, unitPrice);

            // Act
            var result = orderItem1.Equals(orderItem2);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void Equals_DifferentQuantity_ReturnsFalse()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var unitPrice = new Money(10.0m, "SEK");
            var orderItem1 = new OrderItem(productId, 5, unitPrice);
            var orderItem2 = new OrderItem(productId, 10, unitPrice);

            // Act
            var result = orderItem1.Equals(orderItem2);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void Equals_DifferentUnitPrice_ReturnsFalse()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var orderItem1 = new OrderItem(productId, 5, new Money(10.0m, "SEK"));
            var orderItem2 = new OrderItem(productId, 5, new Money(20.0m, "SEK"));

            // Act
            var result = orderItem1.Equals(orderItem2);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void Equals_NullObject_ReturnsFalse()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var unitPrice = new Money(10.0m, "SEK");
            var orderItem = new OrderItem(productId, 5, unitPrice);

            // Act
            var result = orderItem.Equals(null);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void Equals_DifferentType_ReturnsFalse()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var unitPrice = new Money(10.0m, "SEK");
            var orderItem = new OrderItem(productId, 5, unitPrice);
            var otherObject = new { ProductId = productId, Quantity = 5, UnitPrice = unitPrice };

            // Act
            var result = orderItem.Equals(otherObject);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void Constructor_InvalidProductId_ThrowsArgumentException()
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => new OrderItem(Guid.Empty, 5, new Money(10.0m, "SEK")));
        }

        [Fact]
        public void Constructor_InvalidQuantity_ThrowsArgumentException()
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => new OrderItem(Guid.NewGuid(), 0, new Money(10.0m, "SEK")));
        }

        [Fact]
        public void Constructor_InvalidUnitPrice_ThrowsArgumentException()
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => new OrderItem(Guid.NewGuid(), 5, null!));
        }

        [Fact]
        public void UpdateQuantity_ValidQuantity_UpdatesQuantity()
        {
            // Arrange
            var orderItem = new OrderItem(Guid.NewGuid(), 5, new Money(10.0m, "SEK"));

            // Act
            orderItem.UpdateQuantity(10);

            // Assert
            Assert.Equal(10, orderItem.Quantity);
        }

        [Fact]
        public void UpdateQuantity_InvalidQuantity_ThrowsArgumentException()
        {
            // Arrange
            var orderItem = new OrderItem(Guid.NewGuid(), 5, new Money(10.0m, "SEK"));

            // Act & Assert
            Assert.Throws<ArgumentException>(() => orderItem.UpdateQuantity(0));
        }

        [Fact]
public void TotalPrice_CalculatesCorrectly()
{
    // Arrange
    var unitPrice = new Money(10.0m, "SEK");
    var orderItem = new OrderItem(Guid.NewGuid(), 5, unitPrice);

    // Act
    var totalPrice = orderItem.TotalPrice;

    // Assert
    Assert.Equal(new Money(50.0m, "SEK"), totalPrice);
}

        [Fact]
        public void GetHashCode_SameValues_ReturnsSameHashCode()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var unitPrice = new Money(10.0m, "SEK");
            var orderItem1 = new OrderItem(productId, 5, unitPrice);
            var orderItem2 = new OrderItem(productId, 5, unitPrice);

            // Act
            var hashCode1 = orderItem1.GetHashCode();
            var hashCode2 = orderItem2.GetHashCode();

            // Assert
            Assert.Equal(hashCode1, hashCode2);
        }

        [Fact]
        public void GetHashCode_DifferentValues_ReturnsDifferentHashCodes()
        {
            // Arrange
            var orderItem1 = new OrderItem(Guid.NewGuid(), 5, new Money(10.0m, "SEK"));
            var orderItem2 = new OrderItem(Guid.NewGuid(), 10, new Money(20.0m, "SEK"));

            // Act
            var hashCode1 = orderItem1.GetHashCode();
            var hashCode2 = orderItem2.GetHashCode();

            // Assert
            Assert.NotEqual(hashCode1, hashCode2);
        }   
    }
}