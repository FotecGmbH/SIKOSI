// (C) 2020 FOTEC Forschungs- und Technologietransfer GmbH
// Research Subsidiary of FH Wiener Neustadt
// 
// Contact biss@fotec.at / www.fotec.at
// 
// Created 16.12.2020 08:08
// Developer      Roman Jahn
// Project         SIKOSI
// 
// Released under GPL-3.0 or any later version

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.AspNetCore.SignalR.Client;
using Plugin.FilePicker;
using SIKOSI.Crypto.Interfaces;
using SIKOSI.Exchange.Interfaces;
using SIKOSI.Exchange.Model;
using SIKOSI.Sample02.Helpers;
using SIKOSI.SecureServices;
using Xamarin.Forms;
using Thinktecture.Helpers;

namespace SIKOSI.Sample02.ViewModels
{
    /// <summary>
    /// <para>ViewModelChatOverview</para>
    /// Class ViewModelChatOverview. (C) 2020 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public class ViewModelChatOverview : ViewModelBase
    {
        private HubConnection _hubConnection;
        private byte[] _groupKey;
        private readonly Encoding _encoding = Encoding.UTF8;
        private readonly ISecureEncryption _encryption;
        private readonly ISecureSymmetricEncryption _symmetricEncryption;
        private readonly HttpClient _http;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ViewModelChatOverview"/> class.
        /// </summary>
        public ViewModelChatOverview(AuthUserModel currentUser, ISecureEncryption encryption, ISecureSymmetricEncryption symmetricEncryption, HttpClient http)
        {
            CurrentLocalUser = currentUser ?? throw new ArgumentNullException(nameof(currentUser));
            LocalUser = currentUser;
            _encryption = encryption ?? throw new ArgumentNullException(nameof(encryption));
            _symmetricEncryption = symmetricEncryption ?? throw new ArgumentNullException(nameof(symmetricEncryption));
            _http = http ?? throw new ArgumentNullException(nameof(http));

            InitializeCommands();
        }

        #region Properties

        /// <summary>
        /// List of all available chat partners.
        /// </summary>
        public ObservableCollection<User> AvailableUsers { get; set; } = new ObservableCollection<User>();

        /// <summary>
        /// All received group messages.
        /// </summary>
        public ObservableCollection<ExChatMessage> GroupMessages { get; set; } = new ObservableCollection<ExChatMessage>();

        /// <summary>
        /// The list for the picker.
        /// </summary>
        public ObservableCollection<string> PickerList { get; set; } = new ObservableCollection<string> { "Group Chat" };

        /// <summary>
        /// The list of attachments to be sent within the message.
        /// </summary>
        public List<IFile> Attachments { get; set; } = new List<IFile>();

        /// <summary>
        /// Concatenated names of all attachments
        /// </summary>
        public string AttachmentNames => string.Join(", ", Attachments.Select(x => x.Name));

        /// <summary>
        /// The selected User.
        /// </summary>
        public User SelectedUser => (_selectedPickerItemIndex < 1) ? null : AvailableUsers[_selectedPickerItemIndex - 1]; // -1 because first element in PickerList is "Group Chat"
        
        /// <summary>
        /// The backingfield of the selected item of the chat picker.
        /// </summary>
        private int _selectedPickerItemIndex;

        /// <summary>
        /// The selected item of the chat picker.
        /// </summary>
        public int SelectedPickerItemIndex
        {
            get => _selectedPickerItemIndex;
            set
            {
                if (value == _selectedPickerItemIndex) return;

                _selectedPickerItemIndex = value;

                //update viewed messages
                if (SelectedUser is null) PopulateMessageList(GroupMessages);
                else PopulateMessageList(SelectedUser.ExchangedMessages);
            }
        }

        /// <summary>
        /// The text message to send
        /// </summary>
        public string TextToSend { get; set; }

        /// <summary>
        /// Send message command
        /// </summary>
        public ICommand CmdSendMessage { get; set; }

        /// <summary>
        /// Chose Attachment Files
        /// </summary>
        public ICommand CmdChoseAttachments { get; set; }


        /// <summary>
        /// The currently viewed messages
        /// </summary>
        public ObservableCollection<ExChatMessage> ViewedMessages { get; set; } = new ObservableCollection<ExChatMessage>();

        /// <summary>
        /// The event raising when new users where received.
        /// </summary>
        public event EventHandler OnUsersReceived;


        #endregion

        /// <summary>
        /// Initializes the commands of this Viewmodel.
        /// </summary>
        protected void InitializeCommands()
        {
            CmdSendMessage = new Command(() =>
            {
                if (SelectedPickerItemIndex < 1)
                {
                    SendMessage(TextToSend, Attachments);
                    return;
                }

                var user = AvailableUsers[SelectedPickerItemIndex - 1];

                SendMessage(TextToSend, Attachments, user);
            });

            CmdChoseAttachments = new Command(async () =>
            {
                var allowedTypes = new [] {"image/*", "video/*"};

                var file = await CrossFilePicker.Current.PickFile(allowedTypes);

                if (file is null)
                {
                    Attachments.Clear();
                }
                else
                {
                    string fileType = new MimeTypeLookup().GetMimeType(file.FileName);

                    Attachments.Add(new ExFile
                                    {
                                        Content = file.DataArray,
                                        LastModified = DateTime.Now,
                                        Name = file.FileName,
                                        Type = fileType,
                                        Size = file.DataArray.Length
                                    });
                }

                OnPropertyChanged(nameof(Attachments));
                OnPropertyChanged(nameof(AttachmentNames));
            });
        }

        public override async Task OnInitialized()
        {
            if (CurrentLocalUser.Id < 1)
            {
                throw new Exception("Not logged in!");
            }

            _hubConnection = new HubConnectionBuilder()
            .WithUrl(_http.BaseAddress + "chathub", options =>
            {
                options.AccessTokenProvider =
                    async () => CurrentLocalUser.Token;
            })
            .Build();

            _hubConnection.On<byte[]>("ReceiveEncryptedMessage", OnEncryptedMessageReceived);
            _hubConnection.On<User>("NewUserAvailable", OnNewUserAvailable);
            _hubConnection.On("DistributeGroupKey", OnDistributeGroupKey);
            _hubConnection.On<byte[]>("ReceiveEncryptedGroupKey", OnEncryptedGroupKeyReceived);
            _hubConnection.On<byte[]>("ReceiveEncryptedGroupMessage", OnEncryptedGroupMessageReceived);

            await _hubConnection.StartAsync().ConfigureAwait(true);

            // get all users
            var cs = new EncryptedCommunicationService { Encryption = _encryption };

            var getAllUsersResult = await cs.TryEncryptedJsonGet<User[]>(_http, $"api/allusers/{CurrentLocalUser.Id}").ConfigureAwait(true);

            if (!getAllUsersResult.IsServiceSuccessful)
            {
                throw new Exception("Could not get available users!");
            }

            foreach (var user in getAllUsersResult.ResultModel)
            {
                if (AvailableUsers.FirstOrDefault(u => u.Id == user.Id) is null && CurrentLocalUser.Id != user.Id)
                {
                    AvailableUsers.Add(user);
                    PickerList.Add(user.Username);
                }
            }
            
            OnPropertyChanged(nameof(AvailableUsers));
            OnPropertyChanged(nameof(PickerList));
            OnUsersReceived?.Invoke(this, EventArgs.Empty);

            await _hubConnection.SendAsync("RequestEncryptedGroupKeyDistribution");
        }

        /// <summary>
        /// Clears and refills the list of viewed messages with the specified list of messages.
        /// </summary>
        /// <param name="messages">The new specified messages.</param>
        public void PopulateMessageList(IEnumerable<ExChatMessage> messages)
        {
            if (messages is null) return;

            ViewedMessages.Clear();

            foreach (var message in messages)
            {
                ViewedMessages.Add(message);
            }

            OnPropertyChanged(nameof(ViewedMessages));
        }

        /// <summary>
        /// Method to be invoked by signalR when an encrypted personal message was received.
        /// Silent fail, if cannot be decrypted.
        /// </summary>
        /// <param name="receivedEncryptedBytes">The received encrypted bytes.</param>
        public void OnEncryptedMessageReceived(byte[] receivedEncryptedBytes)
        {
            if (receivedEncryptedBytes == null) return;

            // decrypt received bytes
            var decryptionResult = _encryption.DecryptData(receivedEncryptedBytes);

            if (!decryptionResult.Success) return; //wasn't meant for this user to encrypt

            // convert from JSON to ExChatMessage
            var receivedMessage = Newtonsoft.Json.JsonConvert.DeserializeObject<ExChatMessage>(_encoding.GetString(decryptionResult.ResultBytes));

            if (receivedMessage != null && receivedMessage.Author.Id != CurrentLocalUser.Id)
            {
                var chatPartner = AvailableUsers.FirstOrDefault(x => x.Id == receivedMessage.Author.Id);

                if (chatPartner == null) return;

                chatPartner.ExchangedMessages.Add(receivedMessage);

                // add to viewed messages if sender matches selected user
                if (!(SelectedUser is null) && chatPartner.Id == SelectedUser.Id)
                {
                    ViewedMessages.Add(receivedMessage);
                    OnPropertyChanged(nameof(ViewedMessages));
                }
            }
        }

        /// <summary>
        /// The method to be invoked by signalR when a new user are available for chatting.
        /// </summary>
        /// <param name="user">The new available user.</param>
        public void OnNewUserAvailable(User user)
        {
            if (user is null || AvailableUsers is null) return;

            var alreadyKnownUser = AvailableUsers.FirstOrDefault(x => x.Id == user.Id);

            // add or update user's data
            if (alreadyKnownUser is null)
            {
                AvailableUsers.Add(user);
                PickerList.Add(user.Username);
            }
            else
            {
                alreadyKnownUser.FirstName = user.FirstName;
                alreadyKnownUser.LastName = user.LastName;
                alreadyKnownUser.Username = user.Username;
                alreadyKnownUser.PublicKey = user.PublicKey;
            }

            OnPropertyChanged(nameof(AvailableUsers));
            OnPropertyChanged(nameof(PickerList));
        }

        /// <summary>
        /// The method invoked by SignalR when distribution of the group key is demanded.
        /// </summary>
        public async void OnDistributeGroupKey()
        {
            if (_groupKey is null) _groupKey = CreateNewGroupKey();

            foreach (var user in AvailableUsers)
            {
                // encrypt
                var encryptionResult = _encryption.EncryptData(user.PublicKey, _groupKey);

                if (encryptionResult.Success)
                {
                    await _hubConnection.SendAsync("SendEncryptedGroupKey", encryptionResult.ResultBytes);
                }
                else
                {
                    throw encryptionResult.CausingException;
                }
            }
        }

        /// <summary>
        /// The method to be invoked by SignalR when a new encrypted group key was distributed.
        /// Silent fail if group key could not be decrypted.
        /// </summary>
        /// <param name="receivedEncrypredGroupKey">The new encrypted group key.</param>
        public  void OnEncryptedGroupKeyReceived(byte[] receivedEncrypredGroupKey)
        {
            if (receivedEncrypredGroupKey == null) return;

            // decrypt received bytes
            var decryptionResult = _encryption.DecryptData(receivedEncrypredGroupKey);

            if (decryptionResult.Success)
            {
                _groupKey = decryptionResult.ResultBytes;
            }
        }

        /// <summary>
        /// The method to be invoked by SignalR when an encrypted group message was sent to the user.
        /// Silent fail if message could not be decrypted.
        /// </summary>
        /// <param name="receivedEncryptedGroupMessage">The encrypted message.</param>
        public void OnEncryptedGroupMessageReceived(byte[] receivedEncryptedGroupMessage)
        {
            if (receivedEncryptedGroupMessage == null) return;

            // decrypt received bytes
            var decryptionResult = _symmetricEncryption.DecryptData(receivedEncryptedGroupMessage, _groupKey);

            if (!decryptionResult.Success) return; // probably not having the right group key

            // convert from JSON to ExChatMessage
            var receivedMessage = Newtonsoft.Json.JsonConvert.DeserializeObject<ExChatMessage>(_encoding.GetString(decryptionResult.ResultBytes));

            if (receivedMessage != null && receivedMessage.Author.Id != CurrentLocalUser.Id)
            {
                GroupMessages.Add(receivedMessage);
                OnPropertyChanged(nameof(GroupMessages));

                if (SelectedUser is null)
                {
                    ViewedMessages.Add(receivedMessage);
                    OnPropertyChanged(nameof(ViewedMessages));
                }
            }
        }

        /// <summary>
        /// Sends a message encrypted for the specified user. If no user provided (i.e. is null) message will be encrypted with the group key.
        /// </summary>
        /// <param name="message">The message to send.</param>
        /// <param name="files">The list of attached files to send.</param>
        /// <param name="user">The user the message should be encrypted for. If null Message is encrypted with group key.</param>
        public async void SendMessage(string message, IEnumerable<IFile> files = null, User user = null)
        {
            var newChatMessage = new ExChatMessage
            {
                Message = message,
                SentAt = DateTime.UtcNow,
                Author = new User
                {
                    Id = CurrentLocalUser.Id,
                    FirstName = CurrentLocalUser.FirstName,
                    LastName = CurrentLocalUser.LastName,
                    Username = CurrentLocalUser.Username,
                    PublicKey = _encryption.PublicKey
                },
                Attachments = files?.Select(f => f.ToExFile()).ToList()
            };

            if (user == null)
            {
                SendMessageToAll(newChatMessage);
                return;
            }

            // convert message to JSON-string
            var jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(newChatMessage);

            // encrypt
            var encryptionResult = _encryption.EncryptData(user.PublicKey, _encoding.GetBytes(jsonString));

            if (encryptionResult.Success)
            {
                if (_hubConnection.State != HubConnectionState.Connected)
                {
                    await _hubConnection.StartAsync();
                }

                // send to server
                await _hubConnection.SendAsync("SendEncryptedBytes", encryptionResult.ResultBytes);

                // update own chat
                user.ExchangedMessages.Add(newChatMessage);
                ViewedMessages.Add(newChatMessage);
                OnPropertyChanged(nameof(ViewedMessages));
            }
            else
            {
                // Todo error handling
                throw encryptionResult.CausingException;
            }
        }

        /// <summary>
        /// Sends a message encrypted with the group key.
        /// </summary>
        /// <param name="message">The message to send.</param>
        private async void SendMessageToAll(ExChatMessage message)
        {
            // convert message to JSON-string
            var jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(message);

            if (_hubConnection.State != HubConnectionState.Connected)
            {
                await _hubConnection.StartAsync();
            }

            // if no groupkey available until yet, distribute a new one first
            if (_groupKey is null)
            {
                _groupKey = CreateNewGroupKey();
                await _hubConnection.SendAsync("SendEncryptedGroupKey", _groupKey);
            }

            // encrypt
            var encryptionResult = _symmetricEncryption.EncryptData(_encoding.GetBytes(jsonString), _groupKey);

            if (encryptionResult.Success)
            {
                // send to server
                await _hubConnection.SendAsync("SendEncryptedBytesToGroup", encryptionResult.ResultBytes);

                // update own chat view
                GroupMessages.Add(message);
                OnPropertyChanged(nameof(GroupMessages));

                ViewedMessages.Add(message);
                OnPropertyChanged(nameof(ViewedMessages));
            }
            else
            {
                throw encryptionResult.CausingException;
            }
        }
    }
}