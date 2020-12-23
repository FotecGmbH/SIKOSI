// (C) 2020 FOTEC Forschungs- und Technologietransfer GmbH
// Research Subsidiary of FH Wiener Neustadt
// 
// Contact biss@fotec.at / www.fotec.at
// 
// Created 18.12.2020 14:20
// Developer      Roman Jahn
// Project         SIKOSI
// 
// Released under GPL-3.0 or any later version

using System;
using SIKOSI.Exchange.Interfaces;
using SIKOSI.Exchange.Model;

namespace SIKOSI.Sample02.Helpers
{
    /// <summary>
    /// <para>Extensions</para>
    /// Class Extensions. (C) 2020 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public static class Extensions
    {
        public static ExFile ToExFile(this IFile file)
        {
            return new ExFile
                   {
                       Id = file.Id,
                       Name = file.Name,
                       Content = file.Content,
                       Size = file.Size,
                       Type = file.Type,
                       LastModified = file.LastModified
                   };
        }
    }
}