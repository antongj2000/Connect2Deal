using Connect2Deal.Data;
using Connect2Deal.Models;
using Microsoft.EntityFrameworkCore;

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
    }
}