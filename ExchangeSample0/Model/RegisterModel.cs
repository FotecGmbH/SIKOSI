namespace SIKOSI.Exchange.Model
{
    public class RegisterModel
    {
        /// <summary>
        /// Firstname
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Lastname
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Username
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Email
        /// </summary>
        public string EMail { get; set; }

        /// <summary>
        /// Password
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// ? Do we need this?? Confirmation Password
        /// </summary>
        public string ConfirmPassword { get; set; }
    }
}
