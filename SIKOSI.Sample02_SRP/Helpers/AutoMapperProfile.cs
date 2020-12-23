// (C) 2020 FOTEC Forschungs- und Technologietransfer GmbH
// Das Forschungsunternehmen der Fachhochschule Wiener Neustadt
// 
// Kontakt biss@fotec.at / www.fotec.at
// 
// Erstversion vom 18.05.2020 14:31
// Entwickler      Manuel Fasching
// Projekt         SIKOSI

using System;
using AutoMapper;
using SIKOSI.Exchange.Model;
using Sample0_Basic.Entities;

namespace SIKOSI.Sample02_SRP.Helpers
{
    /// <summary>
    /// Profile for Automapper
    /// </summary>
    public class AutoMapperProfile : Profile
    {
        /// <summary>
        /// Standard Constructor
        /// Create the mappings
        /// </summary>
        public AutoMapperProfile()
        {
            CreateMap<User, TblUser>();
            CreateMap<TblUser, User>();
            CreateMap<RegisterModel, User>();
        }
    }
}