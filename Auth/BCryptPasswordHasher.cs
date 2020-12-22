using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace Readible.Auth
{
    public class BCryptPasswordHasher<TUser> : IPasswordHasher<TUser> where TUser : class
    {
        private readonly BCryptPasswordHasherOptions options;

        public BCryptPasswordHasher(IOptions<BCryptPasswordHasherOptions> options = null)
        {
            this.options = options?.Value ?? new BCryptPasswordHasherOptions();
        }

        public string HashPassword(TUser user, string password)
        {
            if (password == null) throw new ArgumentNullException(nameof(password));
            return BCrypt.Net.BCrypt.HashPassword(password, options.WorkFactor, options.EnhancedEntropy);
        }

        public PasswordVerificationResult VerifyHashedPassword(TUser user, string providedPassword, string hashedPassword)
        {
            if (hashedPassword == null) throw new ArgumentNullException(nameof(hashedPassword));
            if (providedPassword == null) throw new ArgumentNullException(nameof(providedPassword));

            var isValid = BCrypt.Net.BCrypt.Verify(providedPassword, hashedPassword, options.EnhancedEntropy);
            return isValid ? PasswordVerificationResult.Success : PasswordVerificationResult.Failed;
        }
    }

    public class BCryptPasswordHasherOptions
    {
        public int WorkFactor { get; } = 10;
        public bool EnhancedEntropy { get; } = false;
    }
}