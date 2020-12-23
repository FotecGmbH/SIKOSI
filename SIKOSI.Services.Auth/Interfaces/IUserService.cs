// (C) 2020 FOTEC Forschungs- und Technologietransfer GmbH
// Das Forschungsunternehmen der Fachhochschule Wiener Neustadt
// 
// Kontakt biss@fotec.at / www.fotec.at
// 
// Erstversion vom 24.11.2020 09:46
// Entwickler      Manuel Fasching
// Projekt         SIKOSI

using System;
using System.Collections.Generic;
using SIKOSI.Exchange.Interfaces;

namespace SIKOSI.Services.Auth.Interfaces
{
    public interface IUserService
    {
        IUser Authenticate(string username, string password);
        IEnumerable<IUser> GetAll();
        IUser GetById(int id);
        IUser Create(IUser user, string password);
        void Update(IUser user, string password = null);
        void Delete(int id);
    }
}