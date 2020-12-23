using SIKOSI.Exchange.Interfaces;
using System.Collections.Generic;
using System.ComponentModel;

namespace SIKOSI.Exchange.Model
{
    public class User : IUser
    {
        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Username
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Firstname
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Lastname
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Password Hash
        /// </summary>
        public byte[] PasswordHash { get; set; }

        /// <summary>
        /// Password Salt
        /// </summary>
        public byte[] PasswordSalt { get; set; }

        /// <summary>
        /// User Role
        /// </summary>
        public string Role { get; set; }

        /// <summary>
        /// The current public key of this user for encrypting data
        /// </summary>
        public byte[] PublicKey {get; set;}

        public List<ExChatMessage> ExchangedMessages { get; set;} = new List<ExChatMessage>();

        public List<ExFile> SavedFiles { get; set; } = new List<ExFile>();
    }
}
