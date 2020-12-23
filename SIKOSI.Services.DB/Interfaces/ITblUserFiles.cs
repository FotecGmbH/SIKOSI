// (C) 2020 FOTEC Forschungs- und Technologietransfer GmbH
// Research Subsidiary of FH Wiener Neustadt
// 
// Contact biss@fotec.at / www.fotec.at
// 
// Created 04.12.2020 12:52
// Developer      Roman Jahn
// Project         SIKOSI
// 
// Released under GPL-3.0 or any later version

using System.Collections.Generic;
using SIKOSI.Exchange.Model;

namespace SIKOSI.Services.DB.Interfaces
{
    public interface ITblUserFiles : ITblUser
    {
        #region Properties

        ICollection<ExFile> Files { get; set; }

        #endregion
    }
}