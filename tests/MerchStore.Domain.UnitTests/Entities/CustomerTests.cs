// filepath: c:\projekt_seven\Merchstore\tests\MerchStore.Domain.UnitTests\Entities\CustomerTests.cs
using System;
using MerchStore.Domain.Entities;
using Xunit;

namespace MerchStore.Domain.UnitTests.Entities
{
    public class CustomerTests
    {
        [Fact]
        public void Constructor_ShouldCreateCustomer_WhenValidInputsAreProvided()
        {
            // Arrange
            var firstName = "John";
            var lastName = "Doe";
            var email = "john.doe@example.com";
            var phoneNumber = "1234567890";
            var address = "123 Main St";
            var city = "Metropolis";
            var postalCode = "12345";

            // Act
            var customer = new Customer(firstName, lastName, email, phoneNumber, address, city, postalCode);

            // Assert
            Assert.Equal(firstName, customer.FirstName);
            Assert.Equal(lastName, customer.LastName);
            Assert.Equal(email, customer.Email);
            Assert.Equal(phoneNumber, customer.PhoneNumber);
            Assert.Equal(address, customer.Address);
            Assert.Equal(city, customer.City);
            Assert.Equal(postalCode, customer.PostalCode);
        }

        [Theory]
        [InlineData(null, "Doe", "john.doe@example.com", "1234567890", "123 Main St", "Metropolis", "12345")]
        [InlineData("", "Doe", "john.doe@example.com", "1234567890", "123 Main St", "Metropolis", "12345")]
        [InlineData("John", null, "john.doe@example.com", "1234567890", "123 Main St", "Metropolis", "12345")]
        [InlineData("John", "", "john.doe@example.com", "1234567890", "123 Main St", "Metropolis", "12345")]
        [InlineData("John", "Doe", null, "1234567890", "123 Main St", "Metropolis", "12345")]
        [InlineData("John", "Doe", "invalid-email", "1234567890", "123 Main St", "Metropolis", "12345")]
        [InlineData("John", "Doe", "john.doe@example.com", null, "123 Main St", "Metropolis", "12345")]
        [InlineData("John", "Doe", "john.doe@example.com", "", "123 Main St", "Metropolis", "12345")]
        [InlineData("John", "Doe", "john.doe@example.com", "1234567890", null, "Metropolis", "12345")]
        [InlineData("John", "Doe", "john.doe@example.com", "1234567890", "", "Metropolis", "12345")]
        [InlineData("John", "Doe", "john.doe@example.com", "1234567890", "123 Main St", null, "12345")]
        [InlineData("John", "Doe", "john.doe@example.com", "1234567890", "123 Main St", "", "12345")]
        [InlineData("John", "Doe", "john.doe@example.com", "1234567890", "123 Main St", "Metropolis", null)]
        [InlineData("John", "Doe", "john.doe@example.com", "1234567890", "123 Main St", "Metropolis", "")]
        public void Constructor_ShouldThrowArgumentException_WhenInvalidInputsAreProvided(
            string firstName, string lastName, string email, string phoneNumber, string address, string city, string postalCode)
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => new Customer(firstName, lastName, email, phoneNumber, address, city, postalCode));
        }
    }
}