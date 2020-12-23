using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MFA_Server.Entities
{
    public class TblUser
    {
        public TblUser()
        {
        }

        public TblUser(string username, string password, int id)
        {
            this.ID = id;
            this.Username = username;
            this.Password = password;
        }

        [Key]
        public int ID
        {
            get;
            set;
        }

        public string Username
        {
            get;
            set;
        }

        public string Password
        {
            get;
            set;
        }
    }
}
