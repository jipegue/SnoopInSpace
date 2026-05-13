namespace SnoopInSpace.Domain.Users
{
    /// <summary>
    /// Represents an application user.
    /// </summary>
    public sealed class User
    {
        /// <summary>
        /// Gets or sets the user identifier.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the email address.
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the password hash.
        /// </summary>
        public string PasswordHash { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the creation date.
        /// </summary>
        public DateTime CreatedAtUtc { get; set; }
    }
}
