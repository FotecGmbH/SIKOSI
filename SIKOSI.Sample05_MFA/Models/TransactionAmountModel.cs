// (C) 2019 FOTEC Forschungs- und Technologietransfer GmbH
// Research Subsidiary of FH Wiener Neustadt
// 
// Contact biss@fotec.at / www.fotec.at
// 
// Created:        16.09.2020 14:18
// Developer:      Gregor Faiman
// Project         SIKOSI
//
// Released under GPL-3.0-only
namespace MFA_QR_CODE.Models
{
    /// <summary>
    /// Model für eine "Banktransaktion" um XSRF zu demonstrieren.
    /// </summary>
    public class TransactionAmountModel
    {
        public TransactionAmountModel()
        {
            this.Amount = 0;
            this.DestinationBankAccount = "Nicht definiert";
            this.SourceBankAccount = "Mein eigener Account";
        }

        public TransactionAmountModel(int amount, string sourceBankAccount, string destinationBankAccount)
        {
            this.Amount = amount;
            this.DestinationBankAccount = destinationBankAccount;
            this.SourceBankAccount = sourceBankAccount;
        }

        /// <summary>
        /// Transaktionsbetrag
        /// </summary>
        public int Amount
        {
            get;
            set;
        }

        /// <summary>
        /// Bankaccount auf den überwiesen wird.
        /// </summary>
        public string DestinationBankAccount
        {
            get;
            set;
        }

        /// <summary>
        /// Bankaccount von dem überwiesen wird.
        /// </summary>
        public string SourceBankAccount
        {
            get;
            set;
        }

        /// <summary>
        /// This is just for show purposes.
        /// </summary>
        public string RedirectionMessage
        {
            get;
            set;
        }
    }
}
