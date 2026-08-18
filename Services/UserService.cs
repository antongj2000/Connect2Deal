using Connect2Deal.Data;
using Connect2Deal.Models;
using Connect2Deal.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.ComponentModel;

namespace Connect2Deal.Services
{
    public class UserService
    {
        private readonly AppDbContext mycontext;

        public UserService(AppDbContext _mycontext)
        {
            mycontext = _mycontext;
        }

        #region Registration of a new User

        public async Task<bool> UsernameTaken(string username) =>
            await mycontext.Users.AnyAsync(u => u.Username == username);

        public async Task<bool> EmailTaken(string email) =>
            await mycontext.Users.AnyAsync(u => u.Email == email);

        public async Task<User> RegisterUser(string firstName, string lastName, string username, string email, string password)
        {
            var newUser = new User
            {
                FirstName = firstName,
                LastName = lastName,
                Username = username,
                Email = email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
            };

            mycontext.Users.Add(newUser);
            await mycontext.SaveChangesAsync();
            return newUser;
        }

        #endregion

        #region Login User

        public async Task<User?> LoginCheck(string username, string password)
        {
            var user = await mycontext.Users.SingleOrDefaultAsync(u => u.Username == username);

            if (user == null)
            {
                return null;
            }

            if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                return null;
            }

            return user;
        }


        #endregion


        #region Settings

        public async Task<string?> SaveProfilePicture(int userId, IFormFile image, string rootPath)
        {
            if (image == null || image.Length == 0)
            {
                return null;
            }

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };

            var extension = Path.GetExtension(image.FileName).ToLower();

            if (!allowedExtensions.Contains(extension))
            {
                return null;
            }

            if (image.Length > 5 * 1024 * 1024)
            {
                return null;
            }

            var pathFolder = Path.Combine(rootPath, "uploads", "users", userId.ToString());

            if (!Directory.Exists(pathFolder))
            {
                Directory.CreateDirectory(pathFolder);
            }

            var fileName = Guid.NewGuid() + extension;
            var fullPath = Path.Combine(pathFolder, fileName);

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await image.CopyToAsync(stream);
            }

            var user = await mycontext.Users.FindAsync(userId);

            if (user == null)
            {
                return null;
            }

            user.ProfileImage = $"/uploads/users/{userId}/{fileName}";
            await mycontext.SaveChangesAsync();

            return user.ProfileImage;

        }

        public async Task<User> getUserById(int userId)
        {
            var user = await mycontext.Users.FindAsync(userId);
            return user;
        }


        public async Task<bool> UpdateProfile(int userId, string firstName, string lastName, string? phoneNumber, string? description)
        {
            var user = await mycontext.Users.FindAsync(userId);

            if (user == null)
            {
                return false;
            }

            user.FirstName = firstName;
            user.LastName = lastName;
            user.PhoneNumber = phoneNumber;
            user.Description = description;

            await mycontext.SaveChangesAsync();

            return true;
        }


        public async Task<bool> ChangePassword(int userId, string currentPassword, string newPassword)
        {
            var user = await mycontext.Users.FindAsync(userId);

            if (user == null)
            {
                return false;
            }

            if (!BCrypt.Net.BCrypt.Verify(currentPassword, user.PasswordHash))
            {
                return false;
            }

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
            await mycontext.SaveChangesAsync();

            return true;
        }

        #endregion
    }
}