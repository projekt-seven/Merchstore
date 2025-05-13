using MerchStore.Domain.Common;

namespace MerchStore.Domain.Entities;
public class User : Entity<Guid>
{
    public string Username { get; private set; } = string.Empty;
    public string Password { get; private set; } = string.Empty; 
    public string Email { get; private set; } = string.Empty;
    public string Role { get; private set; } = string.Empty;
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    // Private constructor for EF Core
    private User() { }

    public User(string username, string password, string email, string role)
    {
        if (string.IsNullOrWhiteSpace(username))
            throw new ArgumentException("Username cannot be empty", nameof(username));

        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("Password cannot be empty", nameof(password));

        if (string.IsNullOrWhiteSpace(email) || !email.Contains("@"))
            throw new ArgumentException("Invalid email address", nameof(email));

        if (string.IsNullOrWhiteSpace(role))
            throw new ArgumentException("Role cannot be empty", nameof(role));

        Username = username;
        Password = password; 
        Email = email;
        Role = role;
    }

    public void UpdatePassword(string newPassword)
    {
        if (string.IsNullOrWhiteSpace(newPassword))
            throw new ArgumentException("Password cannot be empty", nameof(newPassword));

        Password = newPassword; 
    }
}