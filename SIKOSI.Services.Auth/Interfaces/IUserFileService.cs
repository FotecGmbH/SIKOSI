// (C) 2020 FOTEC Forschungs- und Technologietransfer GmbH
// Research Subsidiary of FH Wiener Neustadt
// 
// Contact biss@fotec.at / www.fotec.at
// 
// Created 04.12.2020 12:30
// Developer      Roman Jahn
// Project         SIKOSI
// 
// Released under GPL-3.0 or any later version

using System;
using System.Collections.Generic;
using SIKOSI.Exchange.Interfaces;

namespace SIKOSI.Services.Auth.Interfaces
{
    public interface IUserFileService : IUserService
    {
        IEnumerable<IFile> GetAllUserFiles(int userId);

        void SaveUserFile(int userId, IFile file);

        IFile GetUserFile(int userId, int fileId);
    }
}