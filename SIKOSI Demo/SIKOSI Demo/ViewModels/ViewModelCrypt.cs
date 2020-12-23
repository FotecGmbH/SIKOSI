// (C) 2020 FOTEC Forschungs- und Technologietransfer GmbH
// Research Subsidiary of FH Wiener Neustadt
// 
// Contact biss@fotec.at / www.fotec.at
// 
// Created 09.11.2020 07:43
// Developer      Roman Jahn
// Project         SIKOSI
// 
// Released under GPL-3.0 or any later version

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Input;
using Plugin.FilePicker;
using Plugin.FilePicker.Abstractions;
using SIKOSI.Crypto;
using SIKOSI.Crypto.Interfaces;
using SIKOSI.Exchange.Interfaces;
using SIKOSI.Exchange.Model;
using SIKOSI.Sample02.Helpers;
using SIKOSI.SecureServices;
using Thinktecture.Helpers;
using Xamarin.Forms;

namespace SIKOSI.Sample02.ViewModels
{
    /// <summary>
    /// <para>ViewModelCrypt</para>
    /// Class ViewModelCrypt. (C) 2020 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public class ViewModelCrypt : ViewModelBase
    {
        private readonly HttpClient _http;
        private readonly ISecureEncryption _secureEncryption = new SecureEncryptionDhCrossPlatform();
        private readonly ISecureSymmetricEncryption _secureSymmetricEncryption;
        private ExFile _encryptedFileData;
        private string _fileName;
        private FileData _selectedFile;

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewModelCrypt"/> class.
        /// </summary>
        /// <param name="currentUser">The current user.</param>
        /// <param name="secureSymmetricEncryption">The symmetric encryption used for the file encryption.</param>
        /// <param name="secureEncryption">The asymmetric encryption used for communication to the server.</param>
        /// <param name="http">The http client used for communication to the server.</param>
        public ViewModelCrypt(AuthUserModel currentUser, ISecureSymmetricEncryption secureSymmetricEncryption, ISecureEncryption secureEncryption, HttpClient http)
        {
            CurrentLocalUser = currentUser ?? throw new ArgumentNullException(nameof(currentUser));
            _secureSymmetricEncryption = secureSymmetricEncryption ?? throw new ArgumentNullException(nameof(secureSymmetricEncryption));
            _secureEncryption = secureEncryption ?? throw new ArgumentNullException(nameof(secureEncryption));
            _http = http ?? throw new ArgumentNullException(nameof(http));
        }

        #region Properties

        /// <summary>
        /// The command to pick a file.
        /// </summary>
        public ICommand CmdPickFile { get; set; }

        /// <summary>
        /// The command to encrypt a file.
        /// </summary>
        public ICommand CmdEncryptFile { get; set; }
        
        /// <summary>
        /// The command to send the encrypted file to the server.
        /// </summary>
        public ICommand CmdSendEncryptedFile { get; set; }

        /// <summary>
        /// The command to update the list of files that were saved on the server.
        /// </summary>
        public ICommand CmdUpdateFiles { get; set; }

        /// <summary>
        /// The list of saved files.
        /// </summary>
        public ObservableCollection<ExFile> SavedFiles { get; set; } = new ObservableCollection<ExFile>();

        /// <summary>
        /// The password given by the user.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// The filename.
        /// </summary>
        public string FileName
        {
            get => _fileName;
            set
            {
                _fileName = value;
                OnPropertyChanged();
            }
        }

        #endregion

        /// <summary>
        /// The event when a file was picked by the user.
        /// </summary>
        public event EventHandler FilePicked;

        /// <summary>
        ///     Event for successful encryption.
        /// </summary>
        public event EventHandler EncryptionSuccessful;

        /// <summary>
        ///     Event for not successful encryption.
        /// </summary>
        public event EventHandler EncryptionNotSuccessful;

        /// <summary>
        ///     Event for successful sending file to server.
        /// </summary>
        public event EventHandler SendingEncryptedFileSuccessful;

        /// <summary>
        ///     Event for not successful sending file to server.
        /// </summary>
        public event EventHandler SendingEncryptedFileNotSuccessful;

        public override async Task OnInitialized()
        {
            InitializeCommands();

            UpdateFilesList(await GetSavedFiles());
        }

        /// <summary>
        /// Updates the list of saved files. Clears the list and adds the items of the new list.
        /// </summary>
        /// <param name="newFiles">The new list of saved files.</param>
        public void UpdateFilesList(IEnumerable<IFile> newFiles)
        {
            if (newFiles is null) return;

            SavedFiles.Clear();

            foreach (var newFile in newFiles) SavedFiles.Add(newFile.ToExFile());

            OnPropertyChanged(nameof(SavedFiles));
        }

        /// <summary>
        /// Initializes the commands of this Viewmodel.
        /// </summary>
        private void InitializeCommands()
        {
            CmdPickFile = new Command(async () =>
            {
                var file = await CrossFilePicker.Current.PickFile();

                if (file != null)
                {
                    FileName = file.FileName;
                    _selectedFile = file;

                    FilePicked?.Invoke(this, EventArgs.Empty);
                }
            });

            CmdEncryptFile = new Command(() =>
            {
                var cryptoResult = _secureSymmetricEncryption.EncryptDataWithPassword(_selectedFile.DataArray, Password);

                if (cryptoResult.Success)
                {
                    string fileType = new MimeTypeLookup().GetMimeType(_selectedFile.FileName);

                    _encryptedFileData = new ExFile
                                         {
                                             Content = cryptoResult.ResultBytes,
                                             LastModified = DateTime.Now,
                                             Name = _selectedFile.FileName,
                                             Type = fileType,
                                             Size = _selectedFile.DataArray.Length
                                         };

                    EncryptionSuccessful?.Invoke(this, EventArgs.Empty);
                }
                else
                {
                    EncryptionNotSuccessful?.Invoke(this, EventArgs.Empty);
                }
            });

            CmdSendEncryptedFile = new Command(async () =>
            {
                // get public key of server
                var serverPublicKey = await _http.GetByteArrayAsync("api/publickey");

                // encrypt for server
                // (after decryption, server can read all properties except of content because content is just decryptable with password)
                var cs = new EncryptedCommunicationService {Encryption = _secureEncryption, ReceiverPublicKey = serverPublicKey};

                var postResult = await cs.TryEncryptedJsonPostWithoutResultModel(_encryptedFileData, _http, $"api/postfile/{CurrentLocalUser.Id}");

                if (!postResult.IsServiceSuccessful)
                {
                    SendingEncryptedFileNotSuccessful?.Invoke(this, EventArgs.Empty);
                    return;
                }

                SendingEncryptedFileSuccessful?.Invoke(this, EventArgs.Empty);

                UpdateFilesList(await GetSavedFiles());
            });
            
            CmdUpdateFiles = new Command(async () => { UpdateFilesList(await GetSavedFiles()); });
        }

        /// <summary>
        ///     Retrieves all saved files of this user from the server.
        /// </summary>
        /// <returns>The files retrieved from the server.</returns>
        private async Task<IEnumerable<IFile>> GetSavedFiles()
        {
            // get public key of server
            var serverPublicKey = await _http.GetByteArrayAsync("api/publickey");

            //get saved files
            var cs = new EncryptedCommunicationService {Encryption = _secureEncryption, ReceiverPublicKey = serverPublicKey};

            var getResult = await cs.TryEncryptedJsonGet<List<ExFile>>(_http, $"api/getfilesmetadata/{CurrentLocalUser.Id}");

            if (!getResult.IsServiceSuccessful) return new List<IFile>();

            return getResult.ResultModel;
        }
    }
}