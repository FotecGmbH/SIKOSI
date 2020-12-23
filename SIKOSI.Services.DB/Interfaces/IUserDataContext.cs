// (C) 2020 FOTEC Forschungs- und Technologietransfer GmbH
// Research Subsidiary of FH Wiener Neustadt
// 
// Contact biss@fotec.at / www.fotec.at
// 
// Created 04.12.2020 11:05
// Developer      Roman Jahn
// Project         SIKOSI
// 
// Released under GPL-3.0 or any later version

using Microsoft.EntityFrameworkCore;

namespace SIKOSI.Services.DB.Interfaces
{
    public interface IUserDataContext<TTblUser> where TTblUser : class, ITblUser
    {
        #region Properties

        DbSet<TTblUser> Users { get; set; }

        #endregion
    }
}