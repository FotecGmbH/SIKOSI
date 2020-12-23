// (C) 2020 FOTEC Forschungs- und Technologietransfer GmbH
// Research Subsidiary of FH Wiener Neustadt
// 
// Contact biss@fotec.at / www.fotec.at
// 
// Created 04.12.2020 13:42
// Developer      Roman Jahn
// Project         SIKOSI
// 
// Released under GPL-3.0 or any later version

using Microsoft.EntityFrameworkCore;

namespace SIKOSI.Services.DB.Interfaces
{
    public interface IUserFilesDataContext<TTblUserFile> : IUserDataContext<TTblUserFile> where TTblUserFile : class, ITblUserFiles
    {
        #region Properties

        DbSet<TTblUserFile> UserFiles { get; set; }

        #endregion
    }
}