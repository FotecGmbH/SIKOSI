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

using System.ComponentModel.DataAnnotations;

namespace SRPServerAPI.Model
{
    public class ApplicationUser
    {
        public ApplicationUser(string username, byte[] verifier, byte[] salt)
        {
            this.Verifier = verifier;
            this.Username = username;
            this.Salt = salt;
        }

        [Key]
        public int ID
        {
            get;
            set;
        }

        public byte[] Verifier
        {
            get;
            set;
        }

        public string Username
        {
            get;
            set;
        }

        public byte[] Salt
        {
            get;
            set;
        }
    }
}
