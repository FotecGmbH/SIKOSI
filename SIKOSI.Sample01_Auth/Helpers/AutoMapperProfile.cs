// (C) 2020 FOTEC Forschungs- und Technologietransfer GmbH
// Research Subsidiary of FH Wiener Neustadt
// 
// Contact biss@fotec.at / www.fotec.at
// 
// Created 18.05.2020 14:31
// Developer      Manuel Fasching
// Project         SIKOSI
// 
// Released under GPL-3.0 or any later version

using System;
using AutoMapper;
using Sample0_Basic.Entities;
using SIKOSI.Exchange.Model;

namespace SIKOSI.Sample01_Auth.Helpers
{
    /// <summary>
    ///     Profile for Automapper
    /// </summary>
    public class AutoMapperProfile : Profile
    {
        /// <summary>
        ///     Standard Constructor
        ///     Create the mappings
        /// </summary>
        public AutoMapperProfile()
        {
            CreateMap<User, TblUser>();
            CreateMap<TblUser, User>();
            CreateMap<RegisterModel, User>();
        }
    }
}