using SIKOSI.Exchange.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SIKOSI.Sample04_SignalR.Services
{
    public class UserEventArgs
    {
        public User User { get; set; }

        public UserEventArgs(User user)
        {
            User = user ?? throw new ArgumentNullException(nameof(user));
        }
    }
}
