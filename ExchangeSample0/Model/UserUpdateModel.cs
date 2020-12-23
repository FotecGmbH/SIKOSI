using System;
using System.Collections.Generic;
using System.Text;

namespace SIKOSI.Exchange.Model
{
    public class UserUpdateModel : RegisterModel
    {
        public int Id { get; set; }

        public string OldPassword { get; set; }
    }
}
