using SIKOSI.Exchange.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SIKOSI.Sample04_SignalR.Services
{
    public class UserService
    {
        private List<User> users;

        public UserService()
        {
            users = new List<User>();
        }

        public Encoding encoder {get; set; } = Encoding.UTF8;

        public event EventHandler<UserEventArgs> UserAdded;

        public event EventHandler<UserEventArgs> UserDeleted;

        public void AddUser(User user)
        {
            if (user != null)
            {
                users.Add(user);
                UserAdded?.Invoke(this, new UserEventArgs(user));
            }
        }

        public void DeleteUser(User user)
        {
            if (user != null)
            {
                users.Remove(user);
                UserDeleted?.Invoke(this, new UserEventArgs(user));
            }
        }

        public IEnumerable<User> GetAllUsers()
        {
            return users;
        }

        public User GetUserById(int id)
        {
            return users.FirstOrDefault(x => x.Id == id);
        }

        public int GetNewId()
        {
            return users.Any() ? users.Max(x => x.Id) + 1 : 1;
        }

        public bool SameUserNameExists(string username)
        {
            return users.Any(x => x.Username == username);
        }

        public User GetUser(AuthenticateModel authModel)
        {
            //hash password
            using SHA256 sha256 = SHA256.Create();

            var pwHash = sha256.ComputeHash(encoder.GetBytes(authModel.Password));

            return users.FirstOrDefault(x => x.Username == authModel.Username && x.PasswordHash.SequenceEqual(pwHash));
        }

        public bool TryUpdateUserData(AuthUserModel user)
        {
            if (user == null) return false;

            var updatedUser = users.FirstOrDefault(x => x.Id == user.Id);

            if (updatedUser == null) return false;

            updatedUser.Username = user.Username;
            updatedUser.FirstName = user.FirstName;
            updatedUser.LastName = user.LastName;

            return true;
        }

    }
}
