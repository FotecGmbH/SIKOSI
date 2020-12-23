// (C) 2020 FOTEC Forschungs- und Technologietransfer GmbH
// Research Subsidiary of FH Wiener Neustadt
// 
// Contact biss@fotec.at / www.fotec.at
// 
// Created 21.12.2020 10:00
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
    /// <para>ViewModelRegister</para>
    /// Class ViewModelRegister. (C) 2020 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public class ViewModelRegistration : ViewModelBase
    {
        private readonly EncryptedRegistrationService _sikosiRegistrationService;
        private readonly ISecureEncryption _encryption;
        private readonly HttpClient _http;

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewModelRegistration"/> class.
        /// </summary>
        /// <param name="sikosiRegistrationService">The registration service.</param>
        /// <param name="encryption">The used encryption for communication to the server.</param>
        /// <param name="http">The http client used for communication to the server.</param>
        public ViewModelRegistration(EncryptedRegistrationService sikosiRegistrationService, ISecureEncryption encryption, HttpClient http)
        {
            _sikosiRegistrationService = sikosiRegistrationService ?? throw new ArgumentNullException(nameof(sikosiRegistrationService));
            _encryption = encryption ?? throw new ArgumentNullException(nameof(encryption));
            _http = http ?? throw new ArgumentNullException(nameof(http));
            InitializeCommands();
        }

        /// <summary>
        /// The registration model.
        /// </summary>
        public RegisterModel RegistrationModel { get; set; } = new RegisterModel();

        /// <summary>
        /// Register command
        /// </summary>
        public ICommand RegistrationCommand { get; set; }
        
        /// <summary>
        /// Event for successful registration.
        /// </summary>
        public event EventHandler RegistrationSuccessful;

        /// <summary>
        /// Event for not successful registration.
        /// </summary>
        public event EventHandler RegistrationNotSuccessful;

        public override async Task OnInitialized()
        {
            _sikosiRegistrationService.Encryption = _encryption;
            _sikosiRegistrationService.GetReceiverPublicKeyFunc = () => _http.GetByteArrayAsync("api/publickey");
        }

        private void InitializeCommands()
        {
            RegistrationCommand = new Command(async () =>
            {
                try
                {
                    if (!CheckRegistrationData(RegistrationModel))
                    {
                        RegistrationNotSuccessful?.Invoke(this, EventArgs.Empty);
                        return;
                    }

                    var loginResult = await _sikosiRegistrationService.TryEncryptedRegistrationWithoutResultModel(RegistrationModel, _http, "api/account/encryptedregister");

                    if (loginResult.IsServiceSuccessful)
                    {
                        RegistrationSuccessful?.Invoke(this, EventArgs.Empty);
                        OnPropertyChanged();
                    }
                    else
                    {
                        RegistrationModel.Username = string.Empty;
                        RegistrationModel.FirstName = string.Empty;
                        RegistrationModel.LastName = string.Empty;
                        RegistrationModel.Password = string.Empty;
                        RegistrationModel.ConfirmPassword = string.Empty;
                        RegistrationModel.EMail = string.Empty;

                        RegistrationNotSuccessful?.Invoke(this, EventArgs.Empty);
                    }
                }
                catch (Exception e)
                {
                    RegistrationNotSuccessful?.Invoke(this, EventArgs.Empty);
                }
            });
        }

        /// <summary>
        /// Checks the registration model whether ready to send to the server.
        /// </summary>
        /// <param name="registrationModel">The registration model to check.</param>
        /// <returns>Whether all required data are available.</returns>
        public bool CheckRegistrationData(RegisterModel registrationModel)
        {
            return
                !string.IsNullOrWhiteSpace(registrationModel.Username) && 
                !string.IsNullOrWhiteSpace(registrationModel.FirstName) &&
                !string.IsNullOrWhiteSpace(registrationModel.LastName) &&
                !string.IsNullOrWhiteSpace(registrationModel.Password) &&
                registrationModel.Password == registrationModel.ConfirmPassword;
        }
    }
}