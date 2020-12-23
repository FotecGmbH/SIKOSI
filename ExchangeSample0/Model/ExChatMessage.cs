using System;
using System.Collections.Generic;
using System.Text;

namespace SIKOSI.Exchange.Model
{
    public class ExChatMessage
    {
        /// <summary>
        /// The id of the message.
        /// </summary>
        public int Id {get;set;}

        /// <summary>
        /// The Id of the chat the message belongs to.
        /// </summary>
        public int ChatId {get;set;}

        /// <summary>
        /// The Author of the mssage.
        /// </summary>
        public User Author {get;set;}

        /// <summary>
        /// The content of the message.
        /// </summary>
        public string Message {get;set;}

        /// <summary>
        /// The date and time the message has been sent at. UTC
        /// </summary>
        public DateTime SentAt {get;set;}

        public List<ExFile> Attachments { get; set; }
    }
}
