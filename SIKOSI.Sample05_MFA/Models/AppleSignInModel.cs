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
    public class AppleSignInModel
    {
        public AppleSignInModel(string state, string nonce)
        {
            this.State = state;
            this.Nonce = nonce;
        }

        public string State
        {
            get;
            set;
        }

        public string Nonce
        {
            get;
            set;
        }
    }
}
