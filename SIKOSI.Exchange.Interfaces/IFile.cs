// (C) 2020 FOTEC Forschungs- und Technologietransfer GmbH
// Research Subsidiary of FH Wiener Neustadt
// 
// Contact biss@fotec.at / www.fotec.at
// 
// Created 04.12.2020 12:56
// Developer      Roman Jahn
// Project         SIKOSI
// 
// Released under GPL-3.0 or any later version

using System;

namespace SIKOSI.Exchange.Interfaces
{
    public interface IFile
    {
        #region Properties

        int Id { get; set; }

        byte[] Content { get; set; }

        DateTime LastModified { get; set; }

        string Name { get; set; }

        long Size { get; set; }

        string Type { get; set; }

        #endregion
    }
}