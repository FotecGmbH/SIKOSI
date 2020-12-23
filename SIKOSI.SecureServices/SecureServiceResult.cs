// (C) 2020 FOTEC Forschungs- und Technologietransfer GmbH
// Research Subsidiary of FH Wiener Neustadt
// 
// Contact biss@fotec.at / www.fotec.at
// 
// Created 12.11.2020 12:42
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
    ///     Class <see cref="SecureServiceResult{TResult}" />. (C) 2020 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public class SecureServiceResult<TResult>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="SecureServiceResult{TResult}" /> class.
        ///     Use this constructor to create a non successful service result.
        /// </summary>
        /// <param name="exception">An exception that has been thrown or any new exception to transport error messages.</param>
        public SecureServiceResult(Exception exception)
        {
            Exception = exception;
            IsServiceSuccessful = false;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="SecureServiceResult{TResult}" /> class.
        ///     Use this constructor to create a successful service result.
        /// </summary>
        /// <param name="resultModel">The result data that has been created.</param>
        public SecureServiceResult(TResult resultModel)
        {
            ResultModel = resultModel ?? throw new ArgumentNullException(nameof(resultModel));
            IsServiceSuccessful = true;
        }

        #region Properties

        /// <summary>
        ///     An exception that has been thrown or any new exception to transport error messages.
        /// </summary>
        public Exception Exception { get; }

        /// <summary>
        ///     An indicator whether the service completed successful or not.
        /// </summary>
        public bool IsServiceSuccessful { get; }

        /// <summary>
        ///     The result data that has been created.
        /// </summary>
        public TResult ResultModel { get; }

        #endregion
    }
}