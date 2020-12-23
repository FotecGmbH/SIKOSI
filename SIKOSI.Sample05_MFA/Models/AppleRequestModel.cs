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
    public class AppleRequestModel
    {
        /// <summary>
        /// Property das State Information beinhält. Wird verwendet um XSRF Attacken zu verhindern.
        /// </summary>
        public string state
        {
            get;
            set;
        }

        /// <summary>
        /// Property das Fehlerinformation beinhält. Nur gesetzt, wenn tatsächlich ein Fehler auftritt.
        /// </summary>
        public string error
        {
            get;
            set;
        }

        /// <summary>
        /// Property das User Information beinhält.
        /// </summary>
        public string user
        {
            get;
            set;
        }

        /// <summary>
        /// Property das eine eindeutige User ID enthält.
        /// </summary>
        public string id_token
        {
            get;
            set;
        }

        public string code
        {
            get;
            set;
        }

        /// <summary>
        /// Property das die User Email beinhält.
        /// </summary>
        public string email
        {
            get;
            set;
        }
    }
}
