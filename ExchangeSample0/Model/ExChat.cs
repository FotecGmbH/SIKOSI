using System.Collections.Generic;
using System.ComponentModel;

namespace SIKOSI.Exchange.Model
{
    public class ExChat : INotifyPropertyChanged
    {
        private List<ExChatMessage> _messages = new List<ExChatMessage>();


        /// <summary>
        /// The Id of the chat.
        /// </summary>
        public int Id {get;set;}

        /// <summary>
        /// Subject of change.
        /// </summary>
        public string Guid {get;set;}

        /// <summary>
        /// The messages of the chat. 
        /// </summary>
        public List<ExChatMessage> Messages 
        {
            get => _messages;
            set
            {
                _messages = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Messages)));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
