using Microsoft.EntityFrameworkCore;
using RaceDay.Data;
using RaceDay.Models;

namespace RaceDay.DbConnection
{
    public class UserProfileConnection
    {
        private readonly RaceDayContext _context;

        public UserProfileConnection(RaceDayContext context)
        {
            _context = context;
        }

        // Get user profile
        public User? GetUser(int userId)
        {
            return _context.Users
                .AsNoTracking()
                .FirstOrDefault(user => user.UserId == userId);
        }

        // Update user profile
        public User? UpdateUser(
            int userId,
            string firstName,
            string lastName,
            string emailAddress,
            string username)
        {
            var user = _context.Users
                .FirstOrDefault(user => user.UserId == userId);

            if (user == null)
            {
                return null;
            }

            // Check for duplicate username or email
            bool duplicate = _context.Users.Any(user =>
                user.UserId != userId &&
                (user.username == username ||
                 user.EmailAddress == emailAddress));

            if (duplicate)
            {
                throw new InvalidOperationException(
                    "That username or email address is already in use.");
            }

            user.FirstName = firstName;
            user.LastName = lastName;
            user.EmailAddress = emailAddress;
            user.username = username;

            _context.SaveChanges();

            return GetUser(userId);
        }

        // Change user password
        public bool ChangePassword(
            int userId,
            string currentPassword,
            string newPassword)
        {
            var user = _context.Users
                .FirstOrDefault(user => user.UserId == userId);

            if (user == null)
            {
                return false;
            }

            string storedPassword = user.Password;

            bool isBcryptHash =
                storedPassword.StartsWith("$2a$") ||
                storedPassword.StartsWith("$2b$") ||
                storedPassword.StartsWith("$2y$");

            bool passwordValid;

            if (isBcryptHash)
            {
                passwordValid = BCrypt.Net.BCrypt.Verify(
                    currentPassword, storedPassword);
            }
            else
            {
                // Support older plain-text passwords
                passwordValid = string.Equals(
                    currentPassword,
                    storedPassword,
                    StringComparison.Ordinal);
            }

            if (!passwordValid)
            {
                return false;
            }

            // Hash the new password before saving
            user.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);

            _context.SaveChanges();

            return true;
        }
    }
}