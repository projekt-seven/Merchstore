using Xunit;
using MerchStore.Domain.ValueObjects;
using MerchStore.Domain.Entities;

namespace MerchStore.Domain.UnitTests.ValueObjects;

public class CustomerInfoTest
{
    [Fact]
    public void Constructor_ShouldCreateCustomerInfo_WhenAllParametersAreValid()
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
        var customerInfo = new CustomerInfo(firstName, lastName, email, phoneNumber, address, city, postalCode);

        // Assert
        Assert.Equal(firstName, customerInfo.FirstName);
        Assert.Equal(lastName, customerInfo.LastName);
        Assert.Equal(email, customerInfo.Email);
        Assert.Equal(phoneNumber, customerInfo.PhoneNumber);
        Assert.Equal(address, customerInfo.Address);
        Assert.Equal(city, customerInfo.City);
        Assert.Equal(postalCode, customerInfo.PostalCode);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Constructor_ShouldThrowArgumentException_WhenFirstNameIsInvalid(string invalidFirstName)
    {
        // Arrange
        var lastName = "Doe";
        var email = "john.doe@example.com";
        var phoneNumber = "1234567890";
        var address = "123 Main St";
        var city = "Metropolis";
        var postalCode = "12345";

        // Act & Assert
        Assert.Throws<ArgumentException>(() => new CustomerInfo(invalidFirstName, lastName, email, phoneNumber, address, city, postalCode));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Constructor_ShouldThrowArgumentException_WhenLastNameIsInvalid(string invalidLastName)
    {
        // Arrange
        var firstName = "John";
        var email = "john.doe@example.com";
        var phoneNumber = "1234567890";
        var address = "123 Main St";
        var city = "Metropolis";
        var postalCode = "12345";

        // Act & Assert
        Assert.Throws<ArgumentException>(() => new CustomerInfo(firstName, invalidLastName, email, phoneNumber, address, city, postalCode));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("invalid-email")]
    public void Constructor_ShouldThrowArgumentException_WhenEmailIsInvalid(string invalidEmail)
    {
        // Arrange
        var firstName = "John";
        var lastName = "Doe";
        var phoneNumber = "1234567890";
        var address = "123 Main St";
        var city = "Metropolis";
        var postalCode = "12345";

        // Act & Assert
        Assert.Throws<ArgumentException>(() => new CustomerInfo(firstName, lastName, invalidEmail, phoneNumber, address, city, postalCode));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Constructor_ShouldThrowArgumentException_WhenPhoneNumberIsInvalid(string invalidPhoneNumber)
    {
        // Arrange
        var firstName = "John";
        var lastName = "Doe";
        var email = "john.doe@example.com";
        var address = "123 Main St";
        var city = "Metropolis";
        var postalCode = "12345";

        // Act & Assert
        Assert.Throws<ArgumentException>(() => new CustomerInfo(firstName, lastName, email, invalidPhoneNumber, address, city, postalCode));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Constructor_ShouldThrowArgumentException_WhenAddressIsInvalid(string invalidAddress)
    {
        // Arrange
        var firstName = "John";
        var lastName = "Doe";
        var email = "john.doe@example.com";
        var phoneNumber = "1234567890";
        var city = "Metropolis";
        var postalCode = "12345";

        // Act & Assert
        Assert.Throws<ArgumentException>(() => new CustomerInfo(firstName, lastName, email, phoneNumber, invalidAddress, city, postalCode));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Constructor_ShouldThrowArgumentException_WhenCityIsInvalid(string invalidCity)
    {
        // Arrange
        var firstName = "John";
        var lastName = "Doe";
        var email = "john.doe@example.com";
        var phoneNumber = "1234567890";
        var address = "123 Main St";
        var postalCode = "12345";

        // Act & Assert
        Assert.Throws<ArgumentException>(() => new CustomerInfo(firstName, lastName, email, phoneNumber, address, invalidCity, postalCode));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Constructor_ShouldThrowArgumentException_WhenPostalCodeIsInvalid(string invalidPostalCode)
    {
        // Arrange
        var firstName = "John";
        var lastName = "Doe";
        var email = "john.doe@example.com";
        var phoneNumber = "1234567890";
        var address = "123 Main St";
        var city = "Metropolis";

        // Act & Assert
        Assert.Throws<ArgumentException>(() => new CustomerInfo(firstName, lastName, email, phoneNumber, address, city, invalidPostalCode));
    }
}
