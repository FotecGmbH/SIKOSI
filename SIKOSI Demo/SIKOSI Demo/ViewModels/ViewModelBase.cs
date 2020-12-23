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

using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Threading.Tasks;
using SIKOSI.Exchange.Model;

namespace SIKOSI.Sample02.ViewModels
{
    /// <summary>
    /// <para>ViewModelBase</para>
    /// Class ViewModelBase. (C) 2020 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>

    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        #region Properties

        /// <summary>
        /// The current local user as static property.
        /// </summary>
        public static AuthUserModel LocalUser { get; set; } = new AuthUserModel();

        /// <summary>
        /// The current local user.
        /// </summary>
        public AuthUserModel CurrentLocalUser { get; set; } = new AuthUserModel();
        
        #endregion
        
        /// <summary>
        /// Invokes the property changed event for the specified property.
        /// </summary>
        /// <param name="propertyName">The specified property that changed.</param>
        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Method to be called after initializing the component.
        /// </summary>
        /// <returns></returns>
        public abstract Task OnInitialized();

        /// <summary>
        /// Creates a new group key for group messages.
        /// </summary>
        /// <returns>The group key.</returns>
        public static byte[] CreateNewGroupKey()
        {
            byte[] result = new byte[32];

            RandomNumberGenerator random = RandomNumberGenerator.Create();
            random.GetBytes(result);

            return result;
        }

        #region Interface Implementations

        /// <summary>
        /// The property change event.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }
}