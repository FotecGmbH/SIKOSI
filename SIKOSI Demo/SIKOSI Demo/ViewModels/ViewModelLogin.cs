// (C) 2020 FOTEC Forschungs- und Technologietransfer GmbH
// Research Subsidiary of FH Wiener Neustadt
// 
// Contact biss@fotec.at / www.fotec.at
// 
// Created 16.12.2020 15:44
// Developer      Roman Jahn
// Project         SIKOSI
// 
// Released under GPL-3.0 or any later version

using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Input;
using SIKOSI.Crypto.Interfaces;
using SIKOSI.Exchange.Model;
using SIKOSI.SecureServices;
using Xamarin.Forms;

namespace SIKOSI.Sample02.ViewModels
{
    /// <summary>
    ///     <para>ViewModelLogin</para>
    ///     Class ViewModelLogin. (C) 2020 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public class ViewModelLogin : ViewModelBase
    {
        private readonly ISecureEncryption _encryption;
        private readonly HttpClient _http;
        private readonly EncryptedLoginService _sikosiLoginService;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ViewModelLogin" /> class.
        /// </summary>
        /// <param name="currentUser">The current user.</param>
        /// <param name="sikosiLoginService">The login service.</param>
        /// <param name="encryption">The encryption used for communication to server.</param>
        /// <param name="http">The http client used for communication to server.</param>
        public ViewModelLogin(AuthUserModel currentUser, EncryptedLoginService sikosiLoginService, ISecureEncryption encryption, HttpClient http)
        {
            CurrentLocalUser = currentUser ?? throw new ArgumentNullException(nameof(currentUser));
            _sikosiLoginService = sikosiLoginService ?? throw new ArgumentNullException(nameof(sikosiLoginService));
            _encryption = encryption ?? throw new ArgumentNullException(nameof(encryption));
            _http = http ?? throw new ArgumentNullException(nameof(http));
            InitializeCommands();
        }

        #region Properties

        /// <summary>
        ///     The authenticate model for username and password.
        /// </summary>
        public AuthenticateModel AuthModel { get; set; } = new AuthenticateModel();

        /// <summary>
        ///     Login command.
        /// </summary>
        public ICommand LoginCommand { get; set; }

        #endregion

        /// <summary>
        ///     Event for successful login.
        /// </summary>
        public event EventHandler LoginSuccessful;

        /// <summary>
        ///     Event for not successful login.
        /// </summary>
        public event EventHandler LoginNotSuccessful;

        public override Task OnInitialized()
        {
            _sikosiLoginService.Encryption = _encryption;
            _sikosiLoginService.GetReceiverPublicKeyFunc = () => _http.GetByteArrayAsync("api/publickey");

            return Task.CompletedTask;
        }

        /// <summary>
        ///     Initializes the commands in this ViewModel.
        /// </summary>
        private void InitializeCommands()
        {
            LoginCommand = new Command(async () =>
            {
                try
                {
                    var loginResult = await _sikosiLoginService.TryEncryptedLogin<AuthenticateModel, AuthUserModel>(AuthModel, _http, "api/account/encryptedlogin");

                    if (loginResult.IsServiceSuccessful)
                    {
                        CurrentLocalUser.Id = loginResult.ResultModel.Id;
                        CurrentLocalUser.Username = loginResult.ResultModel.Username;
                        CurrentLocalUser.FirstName = loginResult.ResultModel.FirstName;
                        CurrentLocalUser.LastName = loginResult.ResultModel.LastName;
                        CurrentLocalUser.Token = loginResult.ResultModel.Token;
                        CurrentLocalUser.RefreshToken = loginResult.ResultModel.RefreshToken;

                        LoginSuccessful?.Invoke(this, EventArgs.Empty);
                        OnPropertyChanged();
                    }
                    else
                    {
                        LoginNotSuccessful?.Invoke(this, EventArgs.Empty);
                    }
                }
                catch (Exception)
                {
                    LoginNotSuccessful?.Invoke(this, EventArgs.Empty);
                }
            });
        }
    }
}