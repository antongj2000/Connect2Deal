using Connect2Deal.Data;
using Connect2Deal.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace Connect2Deal.Services
{


    public class UserService
    {

        private readonly AppDbContext mycontext;

        public UserService(AppDbContext _mycontext)
        {
            mycontext = _mycontext;
        }


        #region Registration of a new user

        public async Task<bool> UsernameTaken(string username) =>
            await mycontext.users.AnyAsync(u => u.username == username);


        public async Task<bool> EmailTaken(string email) =>
            await mycontext.users.AnyAsync(u => u.email == email);


        public async Task<user> RegisterUser(string first_name, string last_name, string username, string email, string password)
        {
            var NewUser = new user
            {
                first_name = first_name,
                last_name = last_name,
                username = username,
                email = email,
                password_hash = BCrypt.Net.BCrypt.HashPassword(password),
            };

            mycontext.users.Add(NewUser);
            await mycontext.SaveChangesAsync();
            return NewUser;

        }
        #endregion



        #region Login User    


        public async Task<user> LoginCheck(string username, string password)
        {
            var user = await mycontext.users.SingleOrDefaultAsync(u => u.username == username);

            if (user == null)
            {
                return null;
            }

            if(BCrypt.Net.BCrypt.Verify(password, user.password_hash))
            {
            return null;
            }

            return user;
        }


    

        

        #endregion




    }
    }
