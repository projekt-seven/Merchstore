using MerchStore.Domain.Common;

namespace MerchStore.Domain.Entities;

/// <summary>
/// Represents a user in the system.
/// </summary>
public class User : Entity<Guid>
{
    /// <summary>
    /// The user's username, used for login or display.
    /// </summary>
    public string Username { get; private set; }

    /// <summary>
    /// The user's email address.
    /// </summary>
    public string Email { get; private set; }

    /// <summary>
    /// The hashed version of the user's password.
    /// </summary>
    public string HashedPassword { get; private set; }

    /// <summary>
    /// Required for EF Core.
    /// </summary>
    private User() {}

    /// <summary>
    /// Creates a new user with the specified data.
    /// </summary>
    /// <param name="username">The chosen username.</param>
    /// <param name="email">The user's email address.</param>
    /// <param name="hashedPassword">The hashed password string.</param>
    public User(string username, string email, string hashedPassword)
        : base(Guid.NewGuid())
    {
        ArgumentException.ThrowIfNullOrEmpty(username, nameof(username));
        ArgumentException.ThrowIfNullOrEmpty(email, nameof(email));
        ArgumentException.ThrowIfNullOrEmpty(hashedPassword, nameof(hashedPassword));

        Username = username;
        Email = email;
        HashedPassword = hashedPassword;
    }
}