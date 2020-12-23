// (C) 2020 FOTEC Forschungs- und Technologietransfer GmbH
// Research Subsidiary of FH Wiener Neustadt
// 
// Contact biss@fotec.at / www.fotec.at
// 
// Created 13.11.2020 11:34
// Developer      Roman Jahn
// Project         SIKOSI
// 
// Released under GPL-3.0 or any later version

using System;

namespace SIKOSI.SecureServices
{
    /// <summary>
    ///     <para>
    ///         This class provides result information of a secure service.
    ///     </para>
    ///     Class <see cref="SecureServiceResultNoContent" />. (C) 2020 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public class SecureServiceResultNoContent
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="SecureServiceResultNoContent" /> class.
        /// </summary>
        /// <param name="exception">
        ///     An exception that has been thrown or any new exception to transport error messages in case of a non successful
        ///     service result.
        ///     Omit this parameter to create a successful service result.
        /// </param>
        public SecureServiceResultNoContent(Exception exception = null)
        {
            Exception = exception;
            IsServiceSuccessful = Exception == null;
        }

        #region Properties

        /// <summary>
        ///     An exception that has been thrown or any new exception to transport error messages.
        ///     Is null if case of a succesfull service result.
        /// </summary>
        public Exception Exception { get; }

        /// <summary>
        ///     An indicator whether the service completed successful or not.
        /// </summary>
        public bool IsServiceSuccessful { get; }

        #endregion
    }
}