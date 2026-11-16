using Microsoft.IdentityModel.Tokens;
using RaceDay.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using RaceDay.Data;
using Microsoft.EntityFrameworkCore;

namespace RaceDay.DbConnection
{
    public class Authenticator
    {
        private readonly IConfiguration configuration;
        private readonly RaceDayContext _context;

        public Authenticator(IConfiguration configuration, RaceDayContext context)
        {
            this.configuration = configuration;
            _context = context;
        }

        //REGISTER
        public string register(
           string firstName,
           string lastName,
           string emailAddress,
           string username,
           string password,
           string role)
        {
            try
            {
                // Hash password before storing it
                string passHash = BCrypt.Net.BCrypt.HashPassword(password);

                var newUser = new User
                {
                    FirstName = firstName,
                    LastName = lastName,
                    EmailAddress = emailAddress,
                    username = username,
                    Password = passHash,
                    Role = role
                };

                _context.Users.Add(newUser);
                _context.SaveChanges();

                return "User registered successfully";
            }
            catch (DbUpdateException)
            {
                return "Unable to register user. The username or email address may already be in use.";
            }
            catch (Exception)
            {
                return "Registration failed. Please try again.";
            }
        }

        //LOGIN
        public string login(string username, string password)
        {
            try
            {
                // Find the user using EF Core
                var user = _context.Users
                    .FirstOrDefault(u => u.username == username);

                if (user == null)
                {
                    return "Invalid username or password.";
                }

                string storedPassword = user.Password;

                bool isBcryptHash =
                    storedPassword.StartsWith("$2a$") ||
                    storedPassword.StartsWith("$2b$") ||
                    storedPassword.StartsWith("$2y$");

                bool passwordValid;

                if (isBcryptHash)
                {
                    passwordValid =
                        BCrypt.Net.BCrypt.Verify(password, storedPassword);
                }
                else
                {
                    // Compatibility with older plain-text passwords
                    passwordValid = string.Equals(
                        password,
                        storedPassword,
                        StringComparison.Ordinal);
                }

                if (!passwordValid)
                {
                    return "Invalid username or password.";
                }

                // Upgrade older plain-text passwords to BCrypt
                if (!isBcryptHash)
                {
                    user.Password = BCrypt.Net.BCrypt.HashPassword(password);
                    _context.SaveChanges();
                }

                string jwtKey = configuration["Jwt:Key"]
                    ?? throw new InvalidOperationException(
                        "JWT key is not configured.");

                var key = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(jwtKey));

                var credentials = new SigningCredentials(
                    key, SecurityAlgorithms.HmacSha256);

                var claims = new[]
                {
            new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new Claim(ClaimTypes.Name, user.username),
            new Claim(ClaimTypes.Role, user.Role),
            new Claim("firstName", user.FirstName),
            new Claim("lastName", user.LastName)
        };

                var token = new JwtSecurityToken(
                    issuer: "RaceDay",
                    audience: "RaceDayUsers",
                    claims: claims,
                    expires: DateTime.UtcNow.AddHours(1),
                    signingCredentials: credentials);

                return new JwtSecurityTokenHandler().WriteToken(token);
            }
            catch (Exception)
            {
                return "Login failed. Please try again.";
            }
        }

    }
}