// (C) 2020 FOTEC Forschungs- und Technologietransfer GmbH
// Das Forschungsunternehmen der Fachhochschule Wiener Neustadt
// 
// Kontakt biss@fotec.at / www.fotec.at
// 
// Erstversion vom 24.11.2020 09:56
// Entwickler      Manuel Fasching
// Projekt         SIKOSI

using System;

namespace SIKOSI.Exchange.Interfaces
{
    public interface IToken
    {
        /// <summary>
        /// Token
        /// </summary>
        string Token { get; }
    }
}